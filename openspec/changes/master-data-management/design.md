## Context

LitXusCount is a ground-up rewrite of InventoryMSNV, an ASP.NET Core MVC monolith. The four master data entities (GlAccount, Customer, Supplier, Product) are the foundation that all future transactional modules depend on — Sales invoices reference Customers and Products, Purchase orders reference Suppliers and Products, and all GL postings reference GlAccounts.

The existing LitXusCount foundation (Phases 1–2) established Clean Architecture, EF Core/PostgreSQL, JWT+permission auth, and the `ServiceResult<T>` / `PagedQuery` / `PagedResult<T>` patterns. This change follows those patterns exactly.

Reference system: InventoryMSNV at `C:\tmpProject\InventoryMSNV_20241030` — business rules and field definitions are sourced from `CustomerInfo.cs`, `Supplier` model, `Items` model, `PaymentController`, `ISalesService`, and `IPurchaseService`.

## Goals / Non-Goals

**Goals:**
- Implement all 4 master data entities end-to-end (domain → API → frontend)
- Mirror InventoryMSNV business rules (field names, GL account linkage, pricing tiers)
- Follow established LitXusCount patterns exactly — no new patterns introduced
- All 4 entities usable as FK targets in future Sales/Purchasing/Inventory modules

**Non-Goals:**
- GL posting logic (belongs in a future Accounting module)
- Stock quantity tracking (belongs in Inventory module)
- Invoice/PO creation (belongs in Sales/Purchasing modules)
- Data migration from InventoryMSNV's database (separate migration phase)

## Decisions

### 1. GL Account as prerequisite for Customer/Supplier/Product
**Decision:** GlAccount must be seeded/created before Customers, Suppliers, or Products can be created — FK constraints enforce this at the DB level with `DeleteBehavior.Restrict`.

**Rationale:** InventoryMSNV enforces this rule in code (aborts invoice posting if customer has no GL code). Moving it to a DB constraint catches it earlier and makes the dependency explicit.

**Alternative considered:** Allow null GL account FK and validate at service layer only — rejected because it allows orphaned records that break GL posting later.

### 2. Four GL account FKs on Product
**Decision:** Product carries 4 separate nullable GL account FKs: `SalesRevenueGlAccountId`, `CogsGlAccountId`, `PurchaseApGlAccountId`, `PurchaseInventoryGlAccountId`.

**Rationale:** InventoryMSNV's `Items` model stores 4 GL account codes per product (`CRSALEAC`, `CSALEAC`, `CRPURCHAC`, `CPURCHAC`). These map directly to the 4 journal lines generated during invoice and receipt GL posting. Making them FKs (not free-text codes) ensures referential integrity.

### 3. Pricing tiers stored as flat columns, not a child table
**Decision:** Product has `PriceLevel1`/`PriceLevel2`/`PriceLevel3`/`MinQtyLevel2`/`MinQtyLevel3`/`PromoPrice`/`PromoStartDate`/`PromoEndDate` as flat columns.

**Rationale:** InventoryMSNV uses flat columns (`USP`, `USP2`, `USP3`). Only 3 tiers are needed. A child table would add join complexity for a fixed-tier structure.

### 4. GlAccount self-reference (parent hierarchy)
**Decision:** `GlAccount.ParentId` is a nullable FK to `GlAccount.Id`. No depth limit enforced at DB level — application enforces no circular references via service-layer validation.

**Rationale:** Chart of accounts hierarchies are typically 2–3 levels deep in practice. Full recursive CTE traversal is deferred until reporting requires it.

### 5. Frontend: sectioned forms, not tabs
**Decision:** Customer and Product create/edit forms use `<hr>` section dividers within a single scrollable modal, not a tabbed interface.

**Rationale:** The `PaginatedTable`/settings layer pattern already uses modals. Tabs would require a new UI component. Section dividers keep the pattern consistent with minimal additional code.

## Risks / Trade-offs

- **[Risk] GlAccount hierarchy cycles** → Mitigation: Service layer checks that `ParentId != Id` and that the new parent is not itself a descendant of the account being edited.
- **[Risk] Product form complexity** → Mitigation: Four GL account dropdowns are built with a shared helper function to reduce repetition; sections clearly labelled (General, GL Accounts, Pricing, Stock Control).
- **[Risk] FK cascade on soft-delete** → `DeleteBehavior.Restrict` means a GL Account cannot be soft-deleted if active Customers/Suppliers/Products reference it. Mitigation: Service layer returns a descriptive error; future enhancement can show a "used by X records" count.
- **[Risk] AllActive endpoints returning large lists** → GL Accounts, Customers, Suppliers used as dropdown sources fetch all active records unbounded. Mitigation: Acceptable at current scale; replace with server-side search dropdowns if record counts exceed ~500.

## Migration Plan

1. Add EF Core migration for the 4 new tables
2. Migration auto-runs on startup (`Database.MigrateAsync()` — already in `Program.cs`)
3. No data migration required at this stage — tables start empty
4. Rollback: revert migration via `dotnet ef database update <previous-migration>` and drop the change

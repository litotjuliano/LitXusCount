## Why

LitXusCount is migrating from InventoryMSNV (`C:\tmpProject\InventoryMSNV_20241030`), an ASP.NET Core MVC monolith. The four core master data entities — GL Accounts, Customers, Suppliers, and Products — underpin every transactional module (Sales, Purchasing, Inventory, Accounting) and must be established before those modules can be built.

## What Changes

- Add `GlAccount` entity: chart of accounts with type classification (Asset/Liability/Equity/Revenue/Expense/COGS), self-referential parent hierarchy, control account flag, and opening balance
- Add `Customer` entity: customer master with GL account link (AR account), full billing + consignee addresses, payment terms, credit limit, and lock flag
- Add `Supplier` entity: supplier master with GL account link (AP account), address, contact fields, payment terms, and default currency
- Add `Product` entity: product master with category, 4 GL account FKs (COGS, Sales Revenue, Purchase AP, Purchase Inventory), 3-tier pricing + promo pricing, stock control flag, and unit of measure
- Add permissions for all 4 entities (`Settings.GlAccount.*`, `Settings.Customer.*`, `Settings.Supplier.*`, `Settings.Product.*`)
- Add API controllers: `GlAccountsController`, `CustomersController`, `SuppliersController`, `ProductsController`
- Add frontend pages with full CRUD UI (list + create/edit modal) for all 4 entities
- Add "Master Data" sidebar navigation group

## Capabilities

### New Capabilities

- `gl-account-management`: Chart of accounts CRUD — create/edit/soft-delete GL accounts with type, hierarchy, control flag, and opening balance; all-active lookup endpoint for use in dropdowns across Customers, Suppliers, and Products
- `customer-management`: Customer master CRUD — full customer record including GL account link, dual addresses (billing + consignee), payment terms, credit limit, and active/locked status
- `supplier-management`: Supplier master CRUD — supplier record with GL account link, address, contact details, payment terms, and default currency link
- `product-management`: Product master CRUD — product record with category, UOM, 4 GL account links, 3-tier + promo pricing, and stock control flag

### Modified Capabilities

- `admin-dashboard-shell`: Add "Master Data" sidebar group with GL Accounts, Customers, Suppliers, Products entries (permission-gated, same hide-not-disable rule)

## Impact

- **Backend:** 4 new domain entities, EF Core config with filtered unique indexes and FK relationships, 4 service interfaces + implementations, DI registration, 4 API controllers
- **Frontend:** 4 API client modules, 4 TanStack Query hooks, 4 settings layer components, 4 page components, updated sidebar nav, updated permissions constants
- **Database:** New migration adding 4 tables with FK constraints (DeleteBehavior.Restrict) and filtered unique indexes on `Code` per active row
- **Permissions:** 16 new permission strings added to `Permissions.cs` and mirrored in frontend `permissions.ts`
- **Reference system:** Business rules sourced from InventoryMSNV — `CustomerInfo`, `Supplier`, `Items` models and `ISalesService`/`IPurchaseService` service interfaces

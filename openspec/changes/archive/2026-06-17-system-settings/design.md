## Context

The foundation change established the 4-layer backend, JWT auth, and the WowDash-derived admin shell with placeholder nav for Inventory/Sales/Purchasing/Finance/Identity-Admin. This change adds the first real data: a "System Settings" area mirroring InventoryMSNV's sidebar (Company Info, Email Config, Manage Currency, Payment Type, Payment Status, Customer Type, VAT Percentage, Categories, Units of Measure).

Research of InventoryMSNV's equivalent controllers/models (`CompanyInfoController`, `CurrencyController`, `EmailConfigController`, `PaymentTypeController`, `PaymentStatusController`, `CustomerTypeController`, `VatPercentageController`, `CategoriesController`, `UnitsofMeasureController`) found: every entity inherits an `EntityBase` audit pattern (`CreatedDate`/`ModifiedDate`/`CreatedBy`/`ModifiedBy`/`Cancelled`-as-soft-delete); `Currency`/`VatPercentage`/`EmailConfig` carry an `IsDefault` flag that `CompanyInfoController` flips on Edit; no model has a `CompanyId`/`TenantId` field (confirms single-tenant, consistent with the foundation's non-goals); no DB-enforced uniqueness exists anywhere in this area; `EmailConfig.Password` is stored in plaintext; and several FKs (`PaymentType`, `PaymentStatus`, `CustomerType` consumers) are loose `int`/`long` columns rather than real EF-modeled foreign keys. `SystemSettingsController` itself has no backing model — it only renders a static tile/nav page linking to the others.

Two unrelated lookalikes were found and intentionally excluded: `IncomeCategoryController` (a separate Income/Accounting category, not inventory `Categories`) and `UnitManagementController` (carton-to-smallest-unit conversion ratios, not the `UnitsofMeasure` name lookup). Neither appears in the legacy "System Settings" tile page and neither belongs in this change.

## Goals / Non-Goals

**Goals:**
- A working `CompanyInfo` singleton with default-Currency/VAT%/EmailConfig cascade behavior, enforced in one place (an Application service), not scattered across multiple controller methods as in the legacy app.
- 8 working CRUD lookup entities with real DB-enforced uniqueness where the legacy app had none, and a real EF-modeled FK shape ready for Sales/Purchasing/Inventory to reference later.
- `EmailConfig.Password` encrypted at rest, never round-tripped in API responses.
- A real "System Settings" sidebar group in the admin dashboard with working list/create/edit pages for all 9 entities.

**Non-Goals:**
- No multi-tenancy / `CompanyId` scoping on these lookup tables — matches the foundation's single-tenant stance; revisit only if/when multi-tenancy is actually adopted.
- No `IncomeCategory` or `UnitManagement` (carton conversion) — out of scope, unrelated concepts mistakenly adjacent in the legacy codebase.
- No delete-guard ("can't delete a Category that's referenced by an Item") — Sales/Purchasing/Inventory don't exist yet, so there's nothing to reference these lookups yet; this is revisited when those modules are built and real FK references exist.
- No audit history UI (who changed a setting and when) — `CreatedBy`/`ModifiedBy`/`CreatedAt`/`ModifiedAt` columns are captured, but no screen surfaces them yet.

## Decisions

1. **Two capabilities, not nine.** `company-info-settings` (the singleton + cascade behavior) is functionally distinct from `system-lookup-management` (8 entities sharing one CRUD/soft-delete/uniqueness shape). Splitting further (one capability per entity) would multiply near-identical spec files for no behavioral difference; merging them into one would bury the singleton's unique cascade behavior inside a sea of generic CRUD scenarios.

2. **Soft delete via `IsActive`, not the legacy `Cancelled` flag name.** Same mechanism (filter `IsActive == true` in list queries, never physically remove a referenced lookup row), renamed for clarity since `Cancelled` reads as a domain status (e.g., "this currency was cancelled") rather than a delete flag.

3. **Add real unique constraints the legacy app lacked.** `Currency.Code`, `PaymentType.Name`, `PaymentStatus.Name`, `CustomerType.Name`, `Category.Name`, `UnitOfMeasure.Name` each get a unique index (scoped to active rows only, so a soft-deleted "USD" doesn't block creating a new active "USD"). Alternative considered: match legacy exactly (no constraints) — rejected per explicit user direction; duplicate currency codes or category names serve no purpose and only legacy's lack of FK relationships let them go unnoticed.

4. **`IsDefault` cascade lives in an Application-layer `CompanyInfoService`, triggered only by `CompanyInfo` edits.** Mirrors the legacy behavior (editing `CompanyInfo.CurrencyId` unsets the old default and sets the new one) but as a single explicit method per pointer (`SetDefaultCurrency`, `SetDefaultVatPercentage`, `SetDefaultEmailConfig`) rather than logic embedded in 3 different controllers. `Currency`/`VatPercentage`/`EmailConfig`'s own CRUD endpoints do not let a caller set `IsDefault` directly — it is only ever changed via `CompanyInfo`.

5. **`EmailConfig.Password` encrypted with ASP.NET Core Data Protection (`IDataProtector`), stored as ciphertext in `PasswordEncrypted`.** Alternative considered: a dedicated secrets manager (Azure Key Vault, etc.) — rejected as disproportionate for a single SMTP credential on a project with no cloud infrastructure decided yet; Data Protection is built into ASP.NET Core, requires no new infra, and keys can be moved to a persistent store later without changing the application code. The API never returns this field in any GET response (write-only on the DTO).

6. **Real FK columns (`Guid`/`int` foreign keys with EF navigation properties) instead of legacy's loose untyped columns**, e.g. a future `Sale.PaymentTypeId` will be a real FK to `PaymentType`, not an `int` enum-like code as in `PaymentModeHistory.PaymentType`. This change only adds the lookup tables themselves; the FK-consumer side is built when Sales/Purchasing/Inventory are designed, but the lookup tables' primary keys are shaped now to support that.

7. **`SystemSettingsController`'s legacy role (a static tile/nav page) has no backend equivalent.** It is purely the sidebar group label/landing route in the React frontend — no entity, no API endpoint.

## Risks / Trade-offs

- **[Risk] Adding uniqueness constraints that legacy never had could reject data a future migration-from-legacy import would otherwise have accepted (e.g. two legacy currency rows sharing a code).** → Mitigation: not a concern for this change (greenfield, no data migration from InventoryMSNV is planned or in scope); flag this explicitly if a legacy data import is ever proposed later.
- **[Risk] Centralizing the `IsDefault` cascade in `CompanyInfoService` means `Currency`/`VatPercentage`/`EmailConfig` CRUD screens can't offer a "set as default" action of their own.** → Mitigation: acceptable — matches the legacy UX (defaults are only changed from the Company Info screen) and avoids two code paths that could disagree about which row is "the" default.
- **[Trade-off] Real FK shape now, FK-consumer modules later.** Lookup tables are designed for clean future FKs, but nothing references them yet, so referential-integrity behavior (e.g. delete-guards) can't be fully verified until Sales/Purchasing/Inventory exist. Acceptable since this is the standard order: lookups before the modules that consume them.

## Migration Plan

Greenfield — a single new EF Core migration adds all 9 tables (`CompanyInfo`, `Currency`, `VatPercentage`, `EmailConfig`, `PaymentType`, `PaymentStatus`, `CustomerType`, `Category`, `UnitOfMeasure`) plus their unique indexes, applied to the same local SQL Server database the foundation migration created. `CompanyInfo` is seeded with exactly one row (`Id = 1`) as part of the migration or a startup seed step, since the singleton pattern requires that row to always exist.

## Open Questions

- None — InventoryMSNV's behavior for this area was concrete enough that no further research was needed before specs/tasks.

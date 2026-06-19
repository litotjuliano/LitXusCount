## Why

Every later module (Sales, Purchasing, Inventory) depends on a shared set of configuration lookups — currencies, VAT rates, payment types/statuses, customer types, categories, units of measure — plus one company-identity record and outbound email configuration. InventoryMSNV (reference app) proves this out as a "System Settings" area. Building it now, before any transactional module, means Sales/Purchasing/Inventory can reference real lookup data from day one instead of hardcoded values.

## What Changes

- Add a singleton `CompanyInfo` entity (one row, edit-only, no create/delete) holding company identity fields and "default" pointers to a Currency, a VAT Percentage, and an Email Config.
- Add 8 CRUD lookup entities, all soft-deleted and uniquely constrained (unlike the legacy app, which had neither real-FK relationships nor DB-enforced uniqueness): `Currency`, `VatPercentage`, `EmailConfig`, `PaymentType`, `PaymentStatus`, `CustomerType`, `Category`, `UnitOfMeasure`.
- `Currency`, `VatPercentage`, and `EmailConfig` each carry an `IsDefault` flag; setting a row as default via `CompanyInfo`'s edit flips the previous default off — same cascade behavior InventoryMSNV has, but enforced server-side rather than scattered across controller methods.
- `EmailConfig.Password` is encrypted at rest (ASP.NET Core Data Protection) and never returned in API responses — fixes a plaintext-password issue identified in the legacy reference app.
- Add a `/settings` area to the admin dashboard with a sidebar group ("System Settings") containing: Company Info, Email Config, Manage Currency, Payment Type, Payment Status, Customer Type, VAT Percentage, Categories, Units of Measure — each a real list/create/edit page wired to the new API, replacing what was previously just a placeholder concept.
- No "System Settings" backend entity is created — in the reference app it's a static nav page, not data; here it's simply the sidebar group label.

## Capabilities

### New Capabilities
- `company-info-settings`: The singleton `CompanyInfo` entity, its edit-only API, and the default-Currency/VAT%/EmailConfig cascade behavior.
- `system-lookup-management`: The 8 CRUD lookup entities (Currency, VatPercentage, EmailConfig, PaymentType, PaymentStatus, CustomerType, Category, UnitOfMeasure) — shared CRUD/soft-delete/uniqueness pattern, plus EmailConfig's encrypted password handling.

### Modified Capabilities
- `admin-dashboard-shell`: Adds a "System Settings" sidebar group with real (non-placeholder) pages for the above entities, alongside the existing placeholder module entries.

## Impact

- New EF Core entities + migration in `LitXusCount.Domain`/`LitXusCount.Infrastructure`, new `LitXusCount.Application` use cases, new `LitXusCount.API` controllers for all 9 entities.
- New `/settings/*` routes, pages, and API hooks in `Web/admin-dashboard`.
- Establishes the lookup data (Currency, VAT%, PaymentType, PaymentStatus, CustomerType, Category, UnitOfMeasure) that Sales/Purchasing/Inventory modules will reference via real foreign keys going forward.

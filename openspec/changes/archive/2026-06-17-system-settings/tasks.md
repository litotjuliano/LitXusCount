## 1. Domain entities

- [x] 1.1 Add `CompanyInfo` entity to `LitXusCount.Domain` with the fields listed in `company-info-settings` spec, plus `CurrencyId`, `VatPercentageId`, `EmailConfigId` FKs
- [x] 1.2 Add `Currency` entity (`Name`, `Code`, `Symbol`, `Country`, `Description`, `IsDefault`) plus standard audit fields (`CreatedAt`, `ModifiedAt`, `CreatedBy`, `ModifiedBy`, `IsActive`)
- [x] 1.3 Add `VatPercentage` entity (`Name`, `Percentage`, `IsDefault`) plus audit fields
- [x] 1.4 Add `EmailConfig` entity (`Email`, `PasswordEncrypted`, `Hostname`, `Port`, `SslEnabled`, `SenderFullName`, `IsDefault`) plus audit fields
- [x] 1.5 Add `PaymentType`, `PaymentStatus`, `CustomerType`, `Category`, `UnitOfMeasure` entities (`Name`, `Description`) plus audit fields
- [x] 1.6 Confirm `LitXusCount.Domain` still has zero package references after adding these entities (POCOs only)

## 2. EF Core configuration + migration

- [x] 2.1 Add `DbSet<>` properties for all 9 entities to `ApplicationDbContext`
- [x] 2.2 Configure unique indexes (filtered to `IsActive = true`) for `Currency.Code`, `PaymentType.Name`, `PaymentStatus.Name`, `CustomerType.Name`, `Category.Name`, `UnitOfMeasure.Name`
- [x] 2.3 Configure FK relationships from `CompanyInfo` to `Currency`, `VatPercentage`, `EmailConfig`
- [x] 2.4 Create EF Core migration for the 9 new tables and apply it to the local SQL Server database
- [x] 2.5 Add a migration-time or startup seed that ensures exactly one `CompanyInfo` row (`Id = 1`) exists

## 3. Application layer

- [x] 3.1 Add DTOs for each of the 8 lookup entities (list item, create, edit) — `EmailConfig`'s DTOs exclude the password field from any read shape
- [x] 3.2 Add a generic-shaped (but per-entity, no `IRepository<T>`) Application service per lookup entity: list active, get by id, create, edit, soft-delete, with duplicate-name/code validation returning a clear error
- [x] 3.3 Add `CompanyInfoService` with `Get()`, `Edit()`, and the three cascade methods: `SetDefaultCurrency`, `SetDefaultVatPercentage`, `SetDefaultEmailConfig`, each unsetting the previous default and setting the new one in a single transaction
- [x] 3.4 Wire `CompanyInfoService.Edit()` to call the three cascade methods when the corresponding FK changes
- [x] 3.5 Add `IEmailConfigEncryptor` (wrapping `IDataProtector`) used by the `EmailConfig` service to encrypt on write and never decrypt on read (only decrypt where mail is actually sent, out of scope for this change)

## 4. API layer

- [x] 4.1 Add `[Authorize]`-protected REST controllers for the 8 lookup entities: `GET` (list), `GET /{id}`, `POST` (create), `PUT /{id}` (edit), `DELETE /{id}` (soft-delete)
- [x] 4.2 Add `[Authorize]`-protected `CompanyInfoController`: `GET` (the single record), `PUT` (edit) — no `POST`/`DELETE`
- [x] 4.3 Ensure validation errors (duplicate name/code) return a 400 with a clear message, and `EmailConfig` create/edit reject any client-supplied `IsDefault` value
- [x] 4.4 Verify `dotnet build` succeeds with zero errors across all 4 projects

## 5. Backend verification

- [x] 5.1 Create, list, edit, and soft-delete a row for each of the 8 lookup entities via the API (curl/Swagger) and confirm soft-deleted rows disappear from list results
- [x] 5.2 Confirm creating a duplicate active `Currency.Code` (or other unique field) is rejected with a 400
- [x] 5.3 Confirm editing `CompanyInfo.CurrencyId` flips `IsDefault` on the old and new `Currency` rows
- [x] 5.4 Confirm `GET` on `EmailConfig` never returns the password value, and the stored DB value is ciphertext, not plaintext

## 6. Frontend: System Settings pages

- [x] 6.1 Add a "System Settings" sidebar group with entries: Company Info, Email Config, Manage Currency, Payment Type, Payment Status, Customer Type, VAT Percentage, Categories, Units of Measure
- [x] 6.2 Add TanStack Query hooks (via the shared API client) for each of the 8 lookup entities: list, create, edit, delete
- [x] 6.3 Build one reusable list/create/edit page pattern (table + modal or side panel form) and apply it to all 8 lookup entities, varying only the field set
- [x] 6.4 Build the Company Info page: a single edit form (no create/delete), with dropdowns for default Currency/VAT Percentage/Email Config
- [x] 6.5 Surface API validation errors (e.g. duplicate code/name) as inline form errors

## 7. End-to-end verification

- [x] 7.1 Log in, navigate to each of the 9 System Settings pages, and confirm each loads without error
- [x] 7.2 Create, edit, and delete a row through the UI for at least one lookup entity (e.g. Manage Currency) and confirm the change is reflected after a page refresh
- [x] 7.3 Edit Company Info to change the default Currency through the UI and confirm the previously-default Currency's flag is cleared (re-check via API or DB)
- [x] 7.4 Confirm submitting a duplicate name/code through the UI shows a visible inline error and does not navigate away

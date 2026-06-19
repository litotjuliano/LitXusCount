## ADDED Requirements

### Requirement: Company Info is a singleton record
The system SHALL maintain exactly one `CompanyInfo` record. No create or delete operation SHALL exist for this entity — only retrieval and edit of the single existing row.

#### Scenario: Only one Company Info record ever exists
- **WHEN** the database is queried for `CompanyInfo` rows
- **THEN** exactly one row exists, and no API endpoint exists to create an additional row or delete the existing one

#### Scenario: Company Info is seeded on first run
- **WHEN** the system starts against a freshly migrated database with no `CompanyInfo` row
- **THEN** a single default `CompanyInfo` row is created (via migration or startup seed) before any request can read or edit it

### Requirement: Company Info holds default Currency, VAT Percentage, and Email Config pointers
`CompanyInfo` SHALL reference one `Currency`, one `VatPercentage`, and one `EmailConfig` row as the system-wide defaults for each.

#### Scenario: Company Info edit changes the default Currency
- **WHEN** an authorized user edits `CompanyInfo` and selects a different `Currency` than the current default
- **THEN** the previously-default `Currency` row's `IsDefault` flag is set to false and the newly-selected row's `IsDefault` flag is set to true, in the same operation

#### Scenario: Company Info edit changes the default VAT Percentage
- **WHEN** an authorized user edits `CompanyInfo` and selects a different `VatPercentage` than the current default
- **THEN** the previously-default `VatPercentage` row's `IsDefault` flag is set to false and the newly-selected row's `IsDefault` flag is set to true, in the same operation

#### Scenario: Company Info edit changes the default Email Config
- **WHEN** an authorized user edits `CompanyInfo` and selects a different `EmailConfig` than the current default
- **THEN** the previously-default `EmailConfig` row's `IsDefault` flag is set to false and the newly-selected row's `IsDefault` flag is set to true, in the same operation

### Requirement: Company Info fields
`CompanyInfo` SHALL store company identity and invoicing fields: name, logo, address (street, city, country, post code), contact details (phone, mobile, email, fax, website), company/VAT registration numbers, invoice/quote number prefixes, terms and conditions text, and VAT display preferences (whether VAT applies, VAT title, item discount percentage flag).

#### Scenario: Company Info edit persists all fields
- **WHEN** an authorized user submits an edit to `CompanyInfo` with new values for any of its fields
- **THEN** the updated values are persisted and returned on the next retrieval

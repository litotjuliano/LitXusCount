## ADDED Requirements

### Requirement: Shared CRUD lookup entities
The system SHALL provide CRUD management (list, create, edit, soft-delete) for eight lookup entities: `Currency`, `VatPercentage`, `EmailConfig`, `PaymentType`, `PaymentStatus`, `CustomerType`, `Category`, `UnitOfMeasure`. Each entity carries `Name` plus its entity-specific fields (e.g. `Currency.Code`/`Symbol`, `VatPercentage.Percentage`), and standard audit fields (`CreatedAt`, `ModifiedAt`, `CreatedBy`, `ModifiedBy`, `IsActive`).

#### Scenario: A lookup row can be created
- **WHEN** an authorized user submits a create request for one of the eight lookup entities with valid field values
- **THEN** a new row is persisted with `IsActive = true` and the standard audit fields populated

#### Scenario: A lookup row can be edited
- **WHEN** an authorized user submits an edit for an existing, active lookup row
- **THEN** the row's fields are updated and `ModifiedAt`/`ModifiedBy` are refreshed

#### Scenario: Listing only shows active rows
- **WHEN** a list request is made for any of the eight lookup entities
- **THEN** only rows with `IsActive = true` are returned

### Requirement: Soft delete via IsActive
Deleting a lookup row SHALL set `IsActive = false` rather than physically removing the row from the database.

#### Scenario: Delete soft-removes a row
- **WHEN** an authorized user deletes an active lookup row
- **THEN** the row's `IsActive` flag is set to false, the row remains in the database, and it no longer appears in list results

### Requirement: Unique active names per entity
For `Currency` (by `Code`), `PaymentType`, `PaymentStatus`, `CustomerType`, `Category`, and `UnitOfMeasure` (by `Name`), the system SHALL reject creating or editing a row whose key field matches another currently-active row's value (case-insensitive), but SHALL allow that value to be reused if the conflicting row has been soft-deleted.

#### Scenario: Duplicate active value is rejected
- **WHEN** an authorized user attempts to create or edit a row with a `Code`/`Name` matching an existing active row's value
- **THEN** the request is rejected with a validation error and no row is created or modified

#### Scenario: Value can be reused after the original is soft-deleted
- **WHEN** a row with `Code`/`Name` "X" has been soft-deleted, and an authorized user creates a new row with `Code`/`Name` "X"
- **THEN** the creation succeeds

### Requirement: IsDefault is only changed via Company Info
`Currency`, `VatPercentage`, and `EmailConfig` each carry an `IsDefault` flag. Their own create/edit endpoints SHALL NOT accept a direct change to `IsDefault` — this flag is only ever changed as a side effect of editing `CompanyInfo`.

#### Scenario: Direct IsDefault change is ignored or rejected
- **WHEN** an authorized user submits a create or edit request for `Currency`, `VatPercentage`, or `EmailConfig` that includes a value for `IsDefault`
- **THEN** the request either rejects the field or ignores it — `IsDefault` is unaffected by this endpoint

### Requirement: Email Config password is encrypted and write-only
`EmailConfig.Password` SHALL be encrypted at rest and SHALL NOT be included in any API response.

#### Scenario: Password is stored encrypted
- **WHEN** an `EmailConfig` row is created or edited with a password value
- **THEN** the value persisted in the database is ciphertext, not the plaintext password

#### Scenario: Password is never returned
- **WHEN** an `EmailConfig` row is retrieved via list or get-by-id
- **THEN** the response does not include the password value (plaintext or ciphertext)

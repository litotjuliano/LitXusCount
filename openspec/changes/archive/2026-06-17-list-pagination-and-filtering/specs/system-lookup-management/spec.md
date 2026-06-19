## MODIFIED Requirements

### Requirement: Shared CRUD lookup entities
The system SHALL provide CRUD management (list, create, edit, soft-delete) for eight lookup entities: `Currency`, `VatPercentage`, `EmailConfig`, `PaymentType`, `PaymentStatus`, `CustomerType`, `Category`, `UnitOfMeasure`. Each entity carries `Name` plus its entity-specific fields (e.g. `Currency.Code`/`Symbol`, `VatPercentage.Percentage`), and standard audit fields (`CreatedAt`, `ModifiedAt`, `CreatedBy`, `ModifiedBy`, `IsActive`). Listing SHALL follow the standard paginated/sortable/filterable list endpoint contract (`PagedQuery`/`PagedResult<T>`) rather than returning every active row in one response.

#### Scenario: A lookup row can be created
- **WHEN** an authorized user submits a create request for one of the eight lookup entities with valid field values
- **THEN** a new row is persisted with `IsActive = true` and the standard audit fields populated

#### Scenario: A lookup row can be edited
- **WHEN** an authorized user submits an edit for an existing, active lookup row
- **THEN** the row's fields are updated and `ModifiedAt`/`ModifiedBy` are refreshed

#### Scenario: Listing only shows active rows, paged
- **WHEN** a list request is made for any of the eight lookup entities with a given `page`/`pageSize`
- **THEN** only rows with `IsActive = true` are eligible, and the response is a `PagedResult<T>` containing at most `pageSize` of them for the requested `page`, plus the total count of matching active rows

#### Scenario: Listing supports search across name/description
- **WHEN** a list request includes a `search` term matching a subset of a lookup entity's active rows (by name, or by name/code for Currency)
- **THEN** only matching active rows are counted and returned

#### Scenario: Listing supports sorting
- **WHEN** a list request includes a `sortBy` value naming one of the entity's sortable columns (e.g. `name`, or `code` for Currency, or `percentage` for VatPercentage) and a `sortDescending` flag
- **THEN** the returned rows are ordered by that column in the requested direction

## ADDED Requirements

### Requirement: Supplier CRUD with GL account and currency linkage
The system SHALL provide CRUD management (list, create, edit, soft-delete) for `Supplier` entities. Each supplier SHALL carry: `Code` (unique among active suppliers), `Name`, `GlAccountId` (FK to GlAccount — the AP account), `Address`, `Address2`, `City`, `State`, `PostCode`, `Country`, `Phone`, `Email`, `ContactPerson`, `PaymentTermsDays` (integer), `DefaultCurrencyId` (nullable FK to Currency), and standard audit fields. Listing SHALL follow the `PagedQuery`/`PagedResult<T>` contract.

#### Scenario: Supplier is created with valid fields
- **WHEN** an authorized user submits a create request with a unique Code, Name, and valid GlAccountId
- **THEN** a new Supplier row is persisted with `IsActive = true` and audit fields populated

#### Scenario: Duplicate Code among active suppliers is rejected
- **WHEN** an authorized user attempts to create a Supplier with a Code matching an existing active supplier
- **THEN** the request is rejected with a validation error

#### Scenario: Soft delete sets IsActive to false
- **WHEN** an authorized user deletes an active Supplier
- **THEN** `IsActive` is set to false and the supplier no longer appears in list results

#### Scenario: List returns paged active suppliers
- **WHEN** a list request is made with `page=1&pageSize=20`
- **THEN** only `IsActive = true` suppliers are returned in a `PagedResult<T>` envelope

#### Scenario: List supports search by name or code
- **WHEN** a list request includes a `search` term
- **THEN** only suppliers whose Code or Name contains the search term (case-insensitive) are returned

### Requirement: Supplier all-active lookup endpoint
The system SHALL expose a `GET /api/settings/suppliers/all-active` endpoint returning all `IsActive = true` Suppliers (unpaged) for use as dropdown sources in Purchasing module forms.

#### Scenario: All-active endpoint returns full active supplier list
- **WHEN** a GET request is made to `/api/settings/suppliers/all-active`
- **THEN** the response is an array of all active Suppliers with at minimum Id, Code, and Name fields

### Requirement: Per-entity permissions on Supplier endpoints
Supplier endpoints SHALL require `Settings.Supplier.View` for reads and `Settings.Supplier.Create`/`Edit`/`Delete` for writes.

#### Scenario: Missing permission returns 403
- **WHEN** a user without `Settings.Supplier.Create` attempts to create a supplier
- **THEN** the response status is 403

#### Scenario: Holding the permission succeeds
- **WHEN** a user with `Settings.Supplier.Edit` edits an existing supplier
- **THEN** the request succeeds

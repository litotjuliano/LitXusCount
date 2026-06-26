## ADDED Requirements

### Requirement: Customer CRUD with GL account linkage
The system SHALL provide CRUD management (list, create, edit, soft-delete) for `Customer` entities. Each customer SHALL carry: `Code` (unique among active customers), `Name`, `Name2` (optional trade/alternate name), `GlAccountId` (FK to GlAccount — the AR account), billing address fields (`Address`, `Address2`, `City`, `State`, `PostCode`, `Country`), consignee address fields (`ConsigneeName`, `ConsigneeAddress`, `ConsigneeAddress2`, `ConsigneeCity`, `ConsigneeState`, `ConsigneePostCode`, `ConsigneeCountry`), `Phone`, `Email`, `ContactPerson`, `PaymentTermsDays` (integer), `CreditLimit` (decimal), `IsLocked` (boolean), and standard audit fields. Listing SHALL follow the `PagedQuery`/`PagedResult<T>` contract.

#### Scenario: Customer is created with valid fields
- **WHEN** an authorized user submits a create request with a unique Code, Name, and valid GlAccountId
- **THEN** a new Customer row is persisted with `IsActive = true` and audit fields populated

#### Scenario: Duplicate Code among active customers is rejected
- **WHEN** an authorized user attempts to create a Customer with a Code matching an existing active customer
- **THEN** the request is rejected with a validation error

#### Scenario: Soft delete sets IsActive to false
- **WHEN** an authorized user deletes an active Customer
- **THEN** `IsActive` is set to false and the customer no longer appears in list results

#### Scenario: List returns paged active customers
- **WHEN** a list request is made with `page=1&pageSize=20`
- **THEN** only `IsActive = true` customers are returned in a `PagedResult<T>` envelope

#### Scenario: List supports search by name or code
- **WHEN** a list request includes a `search` term
- **THEN** only customers whose Code or Name contains the search term (case-insensitive) are returned

### Requirement: Customer all-active lookup endpoint
The system SHALL expose a `GET /api/settings/customers/all-active` endpoint returning all `IsActive = true` Customers (unpaged) for use as dropdown sources in Sales module forms.

#### Scenario: All-active endpoint returns full active customer list
- **WHEN** a GET request is made to `/api/settings/customers/all-active`
- **THEN** the response is an array of all active Customers with at minimum Id, Code, and Name fields

### Requirement: Per-entity permissions on Customer endpoints
Customer endpoints SHALL require `Settings.Customer.View` for reads and `Settings.Customer.Create`/`Edit`/`Delete` for writes.

#### Scenario: Missing permission returns 403
- **WHEN** a user without `Settings.Customer.View` attempts to list customers
- **THEN** the response status is 403

#### Scenario: Holding the permission succeeds
- **WHEN** a user with `Settings.Customer.Create` creates a customer
- **THEN** the request succeeds

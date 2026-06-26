## ADDED Requirements

### Requirement: Warehouse CRUD
The system SHALL provide CRUD management for `Warehouse` entities. Each warehouse SHALL carry `Code`, `Name`, `Address` (optional), and standard audit fields with `IsActive` soft-delete. Code SHALL be unique among active warehouses. Listing SHALL follow the `PagedQuery`/`PagedResult<T>` contract.

#### Scenario: Warehouse can be created
- **WHEN** an authorized user submits a create request with a unique Code and Name
- **THEN** a new Warehouse row is persisted with IsActive=true

#### Scenario: Duplicate Code is rejected
- **WHEN** an authorized user creates a Warehouse with a Code matching an existing active warehouse
- **THEN** the request is rejected with a validation error

#### Scenario: All-active endpoint returns full warehouse list
- **WHEN** a GET request is made to `/api/settings/warehouses/all-active`
- **THEN** the response is an array of all active Warehouses for use in transfer and item dropdowns

## ADDED Requirements

### Requirement: GL Account CRUD with type classification
The system SHALL provide CRUD management (list, create, edit, soft-delete) for `GlAccount` entities. Each account SHALL carry `Code` (unique among active accounts), `Name`, `GlAccountType` (enum: Asset=1, Liability=2, Equity=3, Revenue=4, Expense=5, Cogs=6), `ParentId` (nullable self-reference), `IsControl` (boolean), and `OpeningBalance` (decimal). Standard audit fields (`CreatedAt`, `ModifiedAt`, `CreatedBy`, `ModifiedBy`, `IsActive`) SHALL be present. Listing SHALL follow the standard `PagedQuery`/`PagedResult<T>` contract.

#### Scenario: GL Account is created with valid fields
- **WHEN** an authorized user submits a create request with a unique Code, Name, and valid GlAccountType
- **THEN** a new GlAccount row is persisted with `IsActive = true` and audit fields populated

#### Scenario: Duplicate Code among active accounts is rejected
- **WHEN** an authorized user attempts to create a GL Account with a Code matching an existing active account
- **THEN** the request is rejected with a validation error

#### Scenario: Code can be reused after original is soft-deleted
- **WHEN** a GL Account with Code "1000" has been soft-deleted and a user creates a new account with Code "1000"
- **THEN** the creation succeeds

#### Scenario: Soft delete sets IsActive to false
- **WHEN** an authorized user deletes an active GL Account that has no active referencing entities
- **THEN** `IsActive` is set to false and the account no longer appears in list results

#### Scenario: Delete is rejected when account is referenced
- **WHEN** an authorized user attempts to delete a GL Account that is referenced by an active Customer, Supplier, or Product
- **THEN** the request is rejected with a descriptive error and the account remains active

#### Scenario: List returns paged active accounts
- **WHEN** a list request is made with `page=1&pageSize=20`
- **THEN** only `IsActive = true` accounts are returned in a `PagedResult<T>` envelope

### Requirement: GL Account self-referential parent hierarchy
The system SHALL allow a `GlAccount` to reference another `GlAccount` as its parent via `ParentId`. The system SHALL reject creating or editing an account whose `ParentId` would create a circular reference (i.e. setting an account's parent to itself or to one of its own descendants).

#### Scenario: Account can be assigned a parent
- **WHEN** a GL Account is created or edited with a valid `ParentId` referencing an existing active account
- **THEN** the parent relationship is persisted

#### Scenario: Circular parent reference is rejected
- **WHEN** an authorized user attempts to set an account's ParentId to its own Id or to a descendant's Id
- **THEN** the request is rejected with a validation error

### Requirement: GL Account all-active lookup endpoint
The system SHALL expose a `GET /api/settings/gl-accounts/all-active` endpoint returning all `IsActive = true` GL Accounts (unpaged) for use as dropdown data sources in Customer, Supplier, and Product forms.

#### Scenario: All-active endpoint returns full active list
- **WHEN** a GET request is made to `/api/settings/gl-accounts/all-active`
- **THEN** the response is an array of all active GL Accounts (no paging envelope)

### Requirement: Per-entity permissions on GL Account endpoints
GL Account endpoints SHALL require `Settings.GlAccount.View` for reads and `Settings.GlAccount.Create`/`Edit`/`Delete` for writes.

#### Scenario: Missing permission returns 403
- **WHEN** a user without `Settings.GlAccount.Create` attempts to create a GL Account
- **THEN** the response status is 403

#### Scenario: Holding the permission succeeds
- **WHEN** a user with `Settings.GlAccount.Edit` edits an existing GL Account
- **THEN** the request succeeds

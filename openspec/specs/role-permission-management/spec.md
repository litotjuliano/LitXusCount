## Purpose

Defines role management, the compile-time permission catalog, role-claim-based permission assignment, the dynamic permission-based authorization mechanism, and the `SuperAdmin` protection rule.

## Requirements

### Requirement: Role CRUD
The system SHALL provide CRUD management for roles (list, create, edit name, delete), built on ASP.NET Core Identity's `IdentityRole` — no separate custom role table.

#### Scenario: A role can be created
- **WHEN** an authorized user submits a create request with a new, unique role name
- **THEN** a new `IdentityRole` is persisted

#### Scenario: A role can be deleted
- **WHEN** an authorized user deletes a role that is not `SuperAdmin`
- **THEN** the role is removed, and users previously assigned to it lose that role's permissions on their next login

### Requirement: Permission catalog and per-role assignment
The system SHALL define a fixed, compile-time catalog of permissions, one View/Create/Edit/Delete set per resource (e.g. `Users.View`, `Users.Create`, `Users.Edit`, `Users.Delete`, `Roles.*`, and one set per business-entity resource such as `Settings.Currency.*`). Permissions SHALL be assignable to roles as Identity role claims (type `"permission"`), with no per-user permission overrides — a user's effective permissions are the union of their assigned roles' permissions.

#### Scenario: A permission can be granted to a role
- **WHEN** an authorized user assigns a catalog permission to a role
- **THEN** a role claim of type `"permission"` with that permission's value is added to the role

#### Scenario: A permission can be revoked from a role
- **WHEN** an authorized user removes a previously-granted permission from a role
- **THEN** the corresponding role claim is removed

#### Scenario: Effective permissions are the union across roles
- **WHEN** a user belongs to two roles with different permission sets
- **THEN** the user's effective permissions are the union of both roles' permissions

### Requirement: SuperAdmin is a protected role
The `SuperAdmin` role SHALL NOT be deletable, and SHALL be treated as having every catalog permission at authorization time regardless of its actual assigned role claims.

#### Scenario: SuperAdmin cannot be deleted
- **WHEN** an authorized user attempts to delete the `SuperAdmin` role
- **THEN** the request is rejected

#### Scenario: SuperAdmin always passes permission checks
- **WHEN** a user who belongs to the `SuperAdmin` role makes a request to an endpoint requiring any catalog permission
- **THEN** the request succeeds regardless of which permission role claims are actually assigned to `SuperAdmin`

### Requirement: Dynamic permission-based authorization
API endpoints SHALL be protectable via `[Authorize(Policy = "Permission.{PermissionName}")]` for any permission in the catalog, without each permission needing to be pre-registered as a named policy in startup configuration.

#### Scenario: Endpoint with required permission rejects a user lacking it
- **WHEN** a user without the `Users.Delete` permission calls an endpoint requiring `[Authorize(Policy = "Permission.Users.Delete")]`
- **THEN** the response status is 403

#### Scenario: Endpoint with required permission accepts a user holding it
- **WHEN** a user with the `Users.Delete` permission calls the same endpoint
- **THEN** the request succeeds

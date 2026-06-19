## Purpose

Defines user account management: CRUD, role assignment, deactivation (not deletion), and admin-triggered password resets.

## Requirements

### Requirement: User CRUD
The system SHALL provide CRUD management for users: list (paginated, per the standard list endpoint contract), create, edit (display name, email, assigned roles), and deactivate. Users are never hard-deleted.

#### Scenario: A user can be created
- **WHEN** an authorized user submits a create request with a valid email, display name, password, and zero or more role names
- **THEN** a new `ApplicationUser` is persisted, the password is stored hashed, and the user is assigned the specified roles

#### Scenario: A user can be edited
- **WHEN** an authorized user submits an edit for an existing user changing their display name, email, or role assignments
- **THEN** the changes are persisted and reflected on the next retrieval

#### Scenario: User list is paginated
- **WHEN** a list request is made for users with a given `page`/`pageSize`
- **THEN** the response is a `PagedResult<T>` containing at most `pageSize` users for the requested page, plus the total matching count

### Requirement: User deactivation, not deletion
Deactivating a user SHALL disable their ability to log in (via Identity lockout set far in the future) rather than removing the `ApplicationUser` row. Reactivating SHALL clear the lockout.

#### Scenario: Deactivated user cannot log in
- **WHEN** an authorized user deactivates an active user, and that user then attempts to log in with correct credentials
- **THEN** the login is rejected as if the account were locked out

#### Scenario: Deactivated user can be reactivated
- **WHEN** an authorized user reactivates a previously-deactivated user
- **THEN** that user can log in again with correct credentials

#### Scenario: Deactivated users are excluded from the active list by default
- **WHEN** the user list is requested without explicitly including inactive users
- **THEN** deactivated users do not appear in the results

### Requirement: Admin-triggered password reset
An authorized user SHALL be able to force a password reset for another user without knowing their current password.

#### Scenario: Admin resets another user's password
- **WHEN** an authorized user submits an admin password reset for a target user with a new password
- **THEN** the target user's password is changed and they can log in with the new password immediately

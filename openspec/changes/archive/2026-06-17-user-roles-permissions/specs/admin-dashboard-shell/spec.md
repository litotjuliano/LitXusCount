## MODIFIED Requirements

### Requirement: Placeholder navigation for planned business modules
The sidebar navigation SHALL include placeholder entries for each planned business module: Inventory, Sales, Purchasing, and Finance — even though their pages contain no real data yet. Identity/Admin is no longer a placeholder (see "Identity/Admin navigation group").

#### Scenario: All planned modules appear in navigation
- **WHEN** the sidebar is rendered
- **THEN** entries for Inventory, Sales, Purchasing, and Finance are all present and clickable, each navigating to a placeholder page

## ADDED Requirements

### Requirement: Identity/Admin navigation group
The sidebar navigation SHALL include a real (non-placeholder) "Identity / Admin" entry containing "Users" and "Roles" pages, each navigating to a working list/edit page wired to the corresponding API.

#### Scenario: Identity/Admin group renders with Users and Roles
- **WHEN** the sidebar is rendered for an authenticated user
- **THEN** an "Identity / Admin" entry is present with "Users" and "Roles" sub-entries, each clickable and navigating to a real page

#### Scenario: Users page lists, creates, edits, and deactivates
- **WHEN** a user with sufficient permissions navigates to the Users page
- **THEN** the page lists existing users fetched from the API, and supports creating, editing, and deactivating/reactivating users through that same API

#### Scenario: Roles page lists, creates, edits permissions, and deletes
- **WHEN** a user with sufficient permissions navigates to the Roles page
- **THEN** the page lists existing roles, and supports creating a role, editing its assigned permissions via a checkbox matrix, and deleting it (except `SuperAdmin`)

### Requirement: Permission-aware UI hides, not disables, unavailable controls
For every System Settings entity and the new Users/Roles pages, the frontend SHALL hide (not show-but-disable) any sidebar entry, Add/Create control, Edit control, or Delete control that the logged-in user's permissions do not allow, based on the `"permission"` claims embedded in their JWT.

#### Scenario: Sidebar entry is hidden without view permission
- **WHEN** a user lacks `Settings.Currency.View`
- **THEN** the "Manage Currency" sidebar entry does not render

#### Scenario: Add control is hidden without create permission
- **WHEN** a user has `Settings.Currency.View` but not `Settings.Currency.Create`
- **THEN** the Manage Currency page shows the existing list but no Add/Create form

#### Scenario: Edit and Delete controls are hidden per-row without the corresponding permission
- **WHEN** a user has `Settings.Currency.View` but neither `Settings.Currency.Edit` nor `Settings.Currency.Delete`
- **THEN** the Manage Currency page's rows show no Edit or Delete buttons

## MODIFIED Requirements

### Requirement: Placeholder navigation for planned business modules
The sidebar navigation SHALL include placeholder entries for each planned business module: Inventory, Sales, Purchasing, and Finance — even though their pages contain no real data yet. Identity/Admin is no longer a placeholder (see "Identity/Admin navigation group"). Master Data is no longer a placeholder (see "Master Data navigation group").

#### Scenario: All planned modules appear in navigation
- **WHEN** the sidebar is rendered
- **THEN** entries for Inventory, Sales, Purchasing, and Finance are all present and clickable, each navigating to a placeholder page

## ADDED Requirements

### Requirement: Master Data navigation group
The sidebar navigation SHALL include a "Master Data" group containing real (non-placeholder) entries: GL Accounts, Customers, Suppliers, and Products — each navigating to a working list/edit page wired to the corresponding API. Each entry SHALL be hidden (not shown-but-disabled) when the logged-in user lacks the corresponding `Settings.{Entity}.View` permission. The entire group SHALL be hidden if the user lacks View permission for all four entities.

#### Scenario: Master Data group renders with all entries for permitted user
- **WHEN** the sidebar is rendered for a user with View permission for all four master data entities
- **THEN** a "Master Data" group is present with GL Accounts, Customers, Suppliers, and Products sub-entries, each clickable and navigating to a real page

#### Scenario: Individual entry is hidden without View permission
- **WHEN** a user lacks `Settings.Customer.View`
- **THEN** the "Customers" sidebar entry does not render, but other Master Data entries the user can view remain visible

#### Scenario: Master Data group is hidden when user has no view permissions
- **WHEN** a user lacks View permission for all four master data entities
- **THEN** the "Master Data" group does not render at all

#### Scenario: Master Data pages list, create, edit, and delete
- **WHEN** a user with sufficient permissions navigates to one of the four Master Data pages (e.g. GL Accounts)
- **THEN** the page lists existing active rows fetched from the API, and supports creating, editing, and soft-deleting rows through that same API

#### Scenario: Add/Edit/Delete controls are hidden without corresponding permission
- **WHEN** a user has `Settings.GlAccount.View` but not `Settings.GlAccount.Create`, `Edit`, or `Delete`
- **THEN** the GL Accounts page shows the existing list but no Add, Edit, or Delete controls

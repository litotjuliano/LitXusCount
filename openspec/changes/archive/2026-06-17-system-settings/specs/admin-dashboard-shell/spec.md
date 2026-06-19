## ADDED Requirements

### Requirement: System Settings navigation group
The sidebar navigation SHALL include a "System Settings" group containing real (non-placeholder) entries: Company Info, Email Config, Manage Currency, Payment Type, Payment Status, Customer Type, VAT Percentage, Categories, and Units of Measure — each navigating to a working list/edit page wired to the corresponding API.

#### Scenario: System Settings group renders with all entries
- **WHEN** the sidebar is rendered for an authenticated user
- **THEN** a "System Settings" group is present with entries for Company Info, Email Config, Manage Currency, Payment Type, Payment Status, Customer Type, VAT Percentage, Categories, and Units of Measure, each clickable and navigating to a real page

#### Scenario: Lookup pages list, create, edit, and delete
- **WHEN** a user navigates to one of the eight CRUD lookup pages (e.g. Manage Currency)
- **THEN** the page lists existing active rows fetched from the API, and supports creating, editing, and deleting rows through that same API

#### Scenario: Company Info page is edit-only
- **WHEN** a user navigates to the Company Info page
- **THEN** the page shows the single existing `CompanyInfo` record and allows editing it, with no create or delete action available

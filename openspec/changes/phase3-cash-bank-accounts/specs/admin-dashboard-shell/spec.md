## MODIFIED Requirements

### Requirement: Placeholder navigation for planned business modules
The sidebar navigation SHALL include placeholder entries for each planned business module: Inventory, Sales, Purchasing, and Finance — even though their pages contain no real data yet. Identity/Admin, Master Data, and Accounts are no longer placeholders.

#### Scenario: All planned modules appear in navigation
- **WHEN** the sidebar is rendered
- **THEN** entries for Inventory, Sales, Purchasing, and Finance are all present and clickable, each navigating to a placeholder page

## ADDED Requirements

### Requirement: Accounts sidebar navigation group
The sidebar SHALL include an "Accounts" group with entries: Accounts, Deposits, Expenses, Transfers — each navigating to a real working page. All entries SHALL be permission-gated (hidden if the user lacks the corresponding View permission).

#### Scenario: Accounts group renders for permitted user
- **WHEN** a user with Accounts View permission views the sidebar
- **THEN** an "Accounts" group with Accounts, Deposits, Expenses, and Transfers entries is visible

#### Scenario: Accounts group hidden without permission
- **WHEN** a user lacks all Accounts View permissions
- **THEN** the Accounts group does not render

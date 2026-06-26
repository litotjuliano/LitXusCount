## MODIFIED Requirements

### Requirement: Placeholder navigation for planned business modules
The sidebar navigation SHALL include a placeholder entry for Finance only. Identity/Admin, Master Data, Accounts, Sales, Purchasing, and Inventory are no longer placeholders.

#### Scenario: Finance placeholder appears in navigation
- **WHEN** the sidebar is rendered
- **THEN** a Finance entry is present and clickable, navigating to a placeholder page

## ADDED Requirements

### Requirement: Inventory sidebar navigation group
The sidebar SHALL replace the Inventory placeholder with a real "Inventory" group containing: Stock Movements, Stock Adjustments, Transfers, Item Requests, Damage Items — each navigating to a real working page. All entries SHALL be permission-gated.

#### Scenario: Inventory group renders for permitted user
- **WHEN** a user with Inventory View permissions views the sidebar
- **THEN** an "Inventory" group with all 5 sub-entries is visible

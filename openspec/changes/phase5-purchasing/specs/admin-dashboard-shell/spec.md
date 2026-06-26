## MODIFIED Requirements

### Requirement: Placeholder navigation for planned business modules
The sidebar navigation SHALL include placeholder entries for each planned business module: Inventory and Finance — even though their pages contain no real data yet. Identity/Admin, Master Data, Accounts, Sales, and Purchasing are no longer placeholders.

#### Scenario: All planned modules appear in navigation
- **WHEN** the sidebar is rendered
- **THEN** entries for Inventory and Finance are all present and clickable, each navigating to a placeholder page

## ADDED Requirements

### Requirement: Purchasing sidebar navigation group
The sidebar SHALL replace the Purchasing placeholder with a real "Purchasing" group containing: Purchase Orders, Purchase Quotes — each navigating to the purchase invoice list filtered by category. All entries SHALL be permission-gated.

#### Scenario: Purchasing group renders with purchase list pages
- **WHEN** a user with Purchasing.View permission views the sidebar
- **THEN** a "Purchasing" group with Purchase Orders and Purchase Quotes entries is visible

## MODIFIED Requirements

### Requirement: Placeholder navigation for planned business modules
All business modules (Inventory, Sales, Purchasing, Finance/Reports) are now implemented. No placeholder navigation entries remain.

#### Scenario: No placeholder pages in navigation
- **WHEN** the sidebar is rendered
- **THEN** all navigation entries lead to real, data-connected pages

## ADDED Requirements

### Requirement: Reports / Finance sidebar navigation group
The sidebar SHALL replace the Finance placeholder with a real "Reports" group containing: Sales Report, Customer Sales Report, Purchases Report, Stock Report, Expense Summary, Income Summary — each navigating to a real report page. All entries SHALL be permission-gated.

#### Scenario: Reports group renders for permitted user
- **WHEN** a user with Reports View permissions views the sidebar
- **THEN** a "Reports" group with all 6 report entries is visible

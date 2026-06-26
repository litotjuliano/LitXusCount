## MODIFIED Requirements

### Requirement: Placeholder navigation for planned business modules
The sidebar navigation SHALL include placeholder entries for each planned business module: Inventory, Purchasing, and Finance — even though their pages contain no real data yet. Identity/Admin, Master Data, Accounts, and Sales are no longer placeholders.

#### Scenario: All planned modules appear in navigation
- **WHEN** the sidebar is rendered
- **THEN** entries for Inventory, Purchasing, and Finance are all present and clickable, each navigating to a placeholder page

## ADDED Requirements

### Requirement: Sales sidebar navigation group
The sidebar SHALL replace the Sales placeholder with a real "Sales" group containing: Invoices, Quotes, Drafts — each navigating to a filtered view of the SalesInvoice list. All entries SHALL be permission-gated.

#### Scenario: Sales group renders with invoice list pages
- **WHEN** a user with Sales.View permission views the sidebar
- **THEN** a "Sales" group with Invoices, Quotes, and Drafts entries is visible, each navigating to the invoice list filtered by category

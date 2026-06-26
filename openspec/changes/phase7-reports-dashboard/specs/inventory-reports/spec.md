## ADDED Requirements

### Requirement: Stock Report
The system SHALL provide a `GET /api/reports/stock` endpoint returning all active Products with current StockQuantity, CostPrice, and computed StockValue (StockQuantity × CostPrice). SHALL support filters: `search` (code/description), `categoryId`, `lowStockOnly` (boolean — returns only products where StockQuantity ≤ a configured low-stock threshold, default 5).

#### Scenario: Stock report returns all active products with quantities
- **WHEN** a GET request is made to `/api/reports/stock`
- **THEN** all active products are returned with their current StockQuantity and StockValue

#### Scenario: Low stock filter returns only low-stock items
- **WHEN** a GET request is made with `lowStockOnly=true`
- **THEN** only products with StockQuantity ≤ 5 are returned

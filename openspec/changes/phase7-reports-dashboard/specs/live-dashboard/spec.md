## ADDED Requirements

### Requirement: Live dashboard summary endpoint
The system SHALL provide a `GET /api/dashboard/summary` endpoint returning a single JSON object with: `totalSalesToday`, `totalSalesThisMonth`, `totalPurchasesThisMonth`, `totalDueFromCustomers`, `totalOwedToSuppliers`, `totalStockValue` (sum of StockQuantity × CostPrice across all active products), and `salesTrend` (array of last 7 days' daily sales totals), `expenseTrend` (array of last 7 days' daily expense totals).

#### Scenario: Dashboard summary returns all KPI fields
- **WHEN** a GET request is made to `/api/dashboard/summary`
- **THEN** the response contains all KPI fields with computed values reflecting current database state

#### Scenario: Trend arrays contain 7 entries
- **WHEN** the dashboard summary is requested
- **THEN** salesTrend and expenseTrend each contain exactly 7 entries (one per day for the last 7 days, including today)

### Requirement: Live dashboard frontend page
The admin dashboard home page SHALL display KPI summary cards and trend charts sourced from the `/api/dashboard/summary` endpoint, replacing the current static placeholder dashboard.

#### Scenario: Dashboard loads with live data
- **WHEN** an authenticated user navigates to the dashboard home
- **THEN** the page displays current KPI values and trend charts fetched from the API

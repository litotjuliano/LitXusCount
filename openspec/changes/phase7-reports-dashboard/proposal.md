## Why

With transactional data from Sales, Purchasing, Inventory, and Accounts all populated, the system needs reporting and a live dashboard to surface business intelligence. This phase migrates all InventoryMSNV report views into LitXusCount's API-driven, React-rendered report pages and replaces the static dashboard with live summary cards and charts.

## What Changes

- Add Sales Report: filterable by date range, customer, payment status; shows invoice totals, paid/due amounts
- Add Customer Sales Report: sales grouped and totalled per customer
- Add Purchases Report: filterable by date range, supplier, payment status
- Add Stock Report: current stock levels per product with low-stock highlighting
- Add Expense Summary Report: expenses grouped by type and period
- Add Income Summary Report: deposits and income grouped by period
- Add Dashboard: live summary cards (total sales, total purchases, total due, stock value) + sales/expense trend charts
- Replace Finance sidebar placeholder with Accounts reports
- Replace existing static dashboard with live data dashboard

## Capabilities

### New Capabilities

- `sales-reports`: Sales Report and Customer Sales Report — server-side filtered, sortable, exportable
- `purchase-reports`: Purchases Report — server-side filtered, sortable
- `inventory-reports`: Stock Report — current quantities, low-stock flag, value at cost
- `financial-summary-reports`: Expense Summary and Income Summary by period
- `live-dashboard`: Summary KPI cards and trend charts sourced from live transactional data

### Modified Capabilities

- `admin-dashboard-shell`: Replace Finance placeholder with real Finance/Reports section; replace static dashboard home with live dashboard

## Impact

- **Backend:** Dedicated read-only report query endpoints (no new entities — queries across existing Sales, Purchasing, Product, AccAccount tables); dashboard summary endpoint
- **Frontend:** 5 report pages with date-range filters and export; dashboard home page with charts (chart library TBD — e.g. Recharts)
- **Database:** No new tables; may add indexes on date/FK columns for report query performance
- **Dependencies:** Requires Phases 3–6 to have meaningful data; all reports are read-only
- **Reference:** InventoryMSNV `SalesReportController.cs`, `PurchasesReportController.cs`, `StockItemReportController.cs`, `CustomerSalesReportController.cs`, `ExpenseReportController.cs`, `IncomeSummaryReportController.cs`, `DashboardController.cs`

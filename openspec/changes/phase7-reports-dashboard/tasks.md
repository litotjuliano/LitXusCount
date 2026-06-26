# Phase 7: Reports & Dashboard — Implementation Tasks

## Section 1: Performance Indexes (Migration)

- [ ] 1.1 Add EF Core migration: `AddReportIndexes` — adds indexes on: `SalesInvoice.CreatedAt`, `SalesInvoice.CustomerId`, `PurchaseInvoice.CreatedAt`, `PurchaseInvoice.SupplierId`, `AccTransaction.CreatedAt`, `AccTransaction.AccountId`

## Section 2: Backend — Dashboard Summary Endpoint

- [ ] 2.1 Create `IDashboardService` with `GetSummaryAsync()` returning `DashboardSummaryDto`
- [ ] 2.2 Define `DashboardSummaryDto` — `totalSalesToday`, `totalSalesThisMonth`, `totalPurchasesThisMonth`, `totalDueFromCustomers`, `totalOwedToSuppliers`, `totalStockValue`, `salesTrend (DailyTotalDto[7])`, `expenseTrend (DailyTotalDto[7])`
- [ ] 2.3 Implement `DashboardService` — direct EF Core queries for each KPI; build trend arrays for last 7 calendar days
- [ ] 2.4 Create `DashboardController` at `Controllers/Dashboard/DashboardController.cs` — GET `/api/dashboard/summary`
- [ ] 2.5 Register `IDashboardService` in `DependencyInjection.cs`

## Section 3: Backend — Sales Reports

- [ ] 3.1 Create `ISalesReportService` with `GetSalesReportAsync(from, to, customerId?, paymentStatusId?)` and `GetCustomerSalesReportAsync(from, to, customerId?)`
- [ ] 3.2 Define `SalesReportRowDto` (InvoiceNo, CustomerName, Date, SubTotal, VATAmount, GrandTotal, PaidAmount, DueAmount, Status) and `SalesReportResponseDto` (Rows, Summary)
- [ ] 3.3 Define `CustomerSalesReportRowDto` (CustomerName, InvoiceCount, TotalGrandTotal, TotalPaid, TotalDue)
- [ ] 3.4 Implement `SalesReportService` with filtered EF Core queries + summary row aggregation
- [ ] 3.5 Create `SalesReportsController` — GET `/api/reports/sales`, GET `/api/reports/customer-sales`

## Section 4: Backend — Purchase Reports

- [ ] 4.1 Create `IPurchaseReportService` with `GetPurchasesReportAsync(from, to, supplierId?, paymentStatusId?)`
- [ ] 4.2 Define `PurchasesReportRowDto` and `PurchasesReportResponseDto`
- [ ] 4.3 Implement `PurchaseReportService`
- [ ] 4.4 Create `PurchaseReportsController` — GET `/api/reports/purchases`

## Section 5: Backend — Stock Report

- [ ] 5.1 Create `IStockReportService` with `GetStockReportAsync(search?, categoryId?, lowStockOnly?)`
- [ ] 5.2 Define `StockReportRowDto` (Code, Description, Category, StockQuantity, CostPrice, StockValue)
- [ ] 5.3 Implement `StockReportService` — returns all active products, computes StockValue per row; lowStockOnly filter uses threshold of 5
- [ ] 5.4 Create `StockReportsController` — GET `/api/reports/stock`

## Section 6: Backend — Financial Summary Reports

- [ ] 6.1 Create `IFinancialReportService` with `GetExpenseSummaryAsync(from, to, groupBy, accountId?)` and `GetIncomeSummaryAsync(from, to, groupBy, accountId?)`
- [ ] 6.2 Define `PeriodTotalDto` (Period, TotalAmount) and response wrappers
- [ ] 6.3 Implement `FinancialReportService` — group by day or month using EF Core date functions
- [ ] 6.4 Create `FinancialReportsController` — GET `/api/reports/expense-summary`, GET `/api/reports/income-summary`
- [ ] 6.5 Register all report services in `DependencyInjection.cs`

## Section 7: Frontend — Install Recharts

- [ ] 7.1 Run `npm install recharts` in `Web/admin-dashboard`
- [ ] 7.2 Confirm Recharts types are available (included in package)

## Section 8: Frontend — API Client Layer

- [ ] 8.1 Create `Web/admin-dashboard/src/api/dashboard.ts` — `getDashboardSummary()`
- [ ] 8.2 Create `src/api/reports/salesReports.ts`, `purchaseReports.ts`, `stockReports.ts`, `financialReports.ts`

## Section 9: Frontend — Hooks

- [ ] 9.1 Create `useDashboard.ts` — `useDashboardSummary()` with 60-second stale time
- [ ] 9.2 Create `useSalesReport.ts`, `usePurchasesReport.ts`, `useStockReport.ts`, `useExpenseSummary.ts`, `useIncomeSummary.ts`

## Section 10: Frontend — Dashboard Home Page

- [ ] 10.1 Replace static placeholder on `DashboardPage.tsx` with live KPI cards: Today's Sales, Month Sales, Month Purchases, Total Due, Total Owed, Stock Value
- [ ] 10.2 Add Sales Trend line chart (Recharts LineChart, last 7 days)
- [ ] 10.3 Add Expense Trend line chart (Recharts LineChart, last 7 days)

## Section 11: Frontend — Report Pages

- [ ] 11.1 Create `SalesReportPage.tsx` — date range picker, customer dropdown filter, PaginatedTable with InvoiceNo/Customer/Date/Totals, summary row at bottom
- [ ] 11.2 Create `CustomerSalesReportPage.tsx` — date range + customer filter, grouped table
- [ ] 11.3 Create `PurchasesReportPage.tsx` — date range + supplier filter, summary row
- [ ] 11.4 Create `StockReportPage.tsx` — search + category filter + low-stock toggle, StockValue column
- [ ] 11.5 Create `ExpenseSummaryPage.tsx` — date range + account filter + groupBy toggle (Daily/Monthly), bar or line chart
- [ ] 11.6 Create `IncomeSummaryPage.tsx` — same shape as Expense Summary

## Section 12: Frontend — Sidebar Navigation

- [ ] 12.1 Replace Finance placeholder with "Reports" group: Sales Report, Customer Sales, Purchases Report, Stock Report, Expense Summary, Income Summary

## Section 13: Permissions

- [ ] 13.1 Add `Reports` static class to `Permissions.cs` — View sub-permission for each report type
- [ ] 13.2 Add `Dashboard` static class — View permission for dashboard summary endpoint

## Section 14: Verification

- [ ] 14.1 `dotnet build` passes
- [ ] 14.2 Migration adds all new indexes
- [ ] 14.3 Dashboard summary endpoint returns all KPI fields with correct values
- [ ] 14.4 Sales Report endpoint returns correct date-filtered results with accurate summary totals
- [ ] 14.5 Customer Sales Report groups correctly
- [ ] 14.6 Purchases Report returns supplier-filtered results
- [ ] 14.7 Stock Report shows StockValue and low-stock filter works
- [ ] 14.8 Expense/Income summary groups by day and month correctly
- [ ] 14.9 Dashboard home page shows live KPI cards and charts after login
- [ ] 14.10 All report pages render with filter UI and correct data
- [ ] 14.11 Reports group visible in sidebar for users with Report.View permissions

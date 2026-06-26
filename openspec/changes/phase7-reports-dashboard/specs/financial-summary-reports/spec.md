## ADDED Requirements

### Requirement: Expense Summary Report
The system SHALL provide a `GET /api/reports/expense-summary` endpoint returning AccExpense records grouped by ExpenseDate period (daily/monthly, controlled by a `groupBy` param), with total amounts per period. SHALL support `from`/`to` date filters and optional `accountId` filter.

#### Scenario: Expense summary groups by month
- **WHEN** a GET request is made with `groupBy=month&from=2026-01-01&to=2026-06-30`
- **THEN** the response contains up to 6 rows (one per month) with total expense amounts

### Requirement: Income Summary Report
The system SHALL provide a `GET /api/reports/income-summary` endpoint returning AccDeposit records grouped by period, with total amounts. Mirrors the expense summary shape and filters.

#### Scenario: Income summary groups by month
- **WHEN** a GET request is made with `groupBy=month`
- **THEN** the response groups deposits by month with correct totals

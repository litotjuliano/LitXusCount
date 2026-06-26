## ADDED Requirements

### Requirement: Sales Report
The system SHALL provide a `GET /api/reports/sales` endpoint returning a list of SalesInvoice records (Regular and Manual categories only, IsActive=true) with joined Customer name, PaymentStatus label, and computed totals. SHALL support filters: `from` (date), `to` (date), `customerId`, `paymentStatusId`. Response is a flat array (not paged) of matching invoices with per-row totals plus a summary row (grand totals across all matching records).

#### Scenario: Sales report returns filtered results
- **WHEN** a GET request is made with `from=2026-01-01&to=2026-06-30`
- **THEN** only invoices within that date range are returned with correct totals

#### Scenario: Sales report summary totals are accurate
- **WHEN** the sales report is queried for a given date range
- **THEN** the summary row SubTotal, VATAmount, GrandTotal, PaidAmount, and DueAmount equal the sum of all returned invoice rows

### Requirement: Customer Sales Report
The system SHALL provide a `GET /api/reports/customer-sales` endpoint returning sales grouped and totalled per customer for a given date range. Each row shows CustomerName, InvoiceCount, TotalSubTotal, TotalGrandTotal, TotalPaid, TotalDue.

#### Scenario: Customer sales report groups by customer
- **WHEN** a GET request is made with a date range covering invoices for 3 customers
- **THEN** the response contains 3 rows, one per customer, with correct aggregated totals

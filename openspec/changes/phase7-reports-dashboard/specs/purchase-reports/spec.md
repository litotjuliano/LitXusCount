## ADDED Requirements

### Requirement: Purchases Report
The system SHALL provide a `GET /api/reports/purchases` endpoint returning PurchaseInvoice records (Regular category, IsActive=true) with joined Supplier name, PaymentStatus label, and computed totals. SHALL support filters: `from` (date), `to` (date), `supplierId`, `paymentStatusId`. Response is a flat array with per-row totals and a summary row.

#### Scenario: Purchases report returns filtered results
- **WHEN** a GET request is made with `supplierId=3`
- **THEN** only purchase invoices for Supplier 3 are returned

#### Scenario: Purchases report summary totals are accurate
- **WHEN** the purchases report is queried
- **THEN** the summary GrandTotal equals the sum of all returned invoice GrandTotal values

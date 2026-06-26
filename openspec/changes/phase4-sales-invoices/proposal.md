## Why

Sales invoicing is the primary revenue-recording workflow in LitXusCount, migrated from InventoryMSNV's `Payment`/`PaymentDetail`/`PaymentModeHistory` system. It covers the full lifecycle: Draft → Quote → Regular invoice, line item management with real-time stock deduction, payment recording against cash/bank accounts, and sales returns.

## What Changes

- Add `SalesInvoice` (header): supports 4 invoice categories — Regular, Draft, Quote, Manual
- Add `SalesInvoiceLine` (line items): per-item quantity, pricing, VAT, discount, return flag
- Add `SalesPaymentRecord`: records how an invoice was paid (cash/bank/cheque); triggers AccAccount balance update
- Add `ReturnLog`: tracks full and partial returns linked to original invoice
- Add stock deduction: every line item save/delete updates `Product.StockQuantity`
- Add invoice number generation: auto-incrementing prefixed sequences per category
- Add printable invoice view
- Add API controllers and frontend invoice management page
- Add "Sales" sidebar module (replaces placeholder)

## Capabilities

### New Capabilities

- `sales-invoice-management`: Full invoice lifecycle — create draft, add/remove lines, promote to regular/quote, record payments, handle returns; stock and account balance updated on every transaction

### Modified Capabilities

- `product-management`: Add `StockQuantity` (integer) field to `Product` entity — current quantity on hand, updated by sales and purchasing transactions
- `admin-dashboard-shell`: Replace Sales placeholder with real Sales Invoice page

## Impact

- **Backend:** 4 new domain entities (SalesInvoice, SalesInvoiceLine, SalesPaymentRecord, ReturnLog), invoice number sequence logic, stock update service, account balance update on payment
- **Frontend:** Invoice list page, invoice editor (header + line items grid + payment recording panel), print view
- **Database:** New migration; `Product` table gains `StockQuantity` column
- **Dependencies:** Requires Phase 3 (AccAccount) for payment recording; requires Phase 2 (Product, Customer) master data
- **Reference:** InventoryMSNV `PaymentController.cs`, `SalesService.cs`, `ConHelper/PaymentService.cs`

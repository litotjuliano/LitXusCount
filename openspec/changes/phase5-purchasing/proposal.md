## Why

Purchasing is the mirror of Sales: where Sales deducts stock and records AR, Purchasing receives stock and records AP. This phase migrates InventoryMSNV's `PurchasesPayment`/`PurchasesPaymentDetail` system, establishing the full PO → Receipt → Payment → Stock-increase → AccAccount flow.

## What Changes

- Add `PurchaseInvoice` (header): mirrors SalesInvoice, linked to Supplier instead of Customer
- Add `PurchaseInvoiceLine` (line items): per-item quantity, pricing, VAT, discount, return flag; stock is **added** (not deducted) on save
- Add `PurchasePaymentRecord`: records payment to supplier; triggers AccAccount balance debit
- Add purchase return support: IsReturn lines, ReturnType on header
- Add purchase invoice number generation: separate sequence from sales (e.g. "PO-" prefix)
- Add printable purchase order/receipt view
- Add API controllers and frontend purchasing page
- Replace Purchasing sidebar placeholder with real page

## Capabilities

### New Capabilities

- `purchase-invoice-management`: Full purchase invoice lifecycle — create, add/remove lines (stock added on save), record supplier payments, handle returns; stock and account balance updated on every transaction

### Modified Capabilities

- `admin-dashboard-shell`: Replace Purchasing placeholder with real Purchase Invoice page

## Impact

- **Backend:** 3 new domain entities (PurchaseInvoice, PurchaseInvoiceLine, PurchasePaymentRecord), purchase number sequence, stock addition logic (shared with Sales stock service)
- **Frontend:** Purchase list page, purchase editor (header + lines + payment panel), print view
- **Database:** New migration for 3 tables
- **Dependencies:** Requires Phase 3 (AccAccount) and Phase 2 (Product, Supplier) master data; stock update service introduced in Phase 4 is reused
- **Reference:** InventoryMSNV `PurchasesPaymentController.cs`, `PurchaseService.cs`

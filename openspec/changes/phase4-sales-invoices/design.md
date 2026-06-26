## Context

InventoryMSNV's sales flow: user opens invoice form → a Draft is auto-created in DB → user adds line items (each triggers stock deduction + invoice total recalc) → user promotes to Regular/Quote → user records payment mode(s) → invoice marked Paid/Partial. Deletions reverse all stock and account movements.

The key complexity is that the invoice is a live, partially-saved document from the moment the form opens — not submitted atomically at the end. LitXusCount must replicate this stateful editing pattern via API calls from the React frontend.

## Goals / Non-Goals

**Goals:**
- Full invoice lifecycle: Draft → Regular/Quote/Manual
- Real-time line item management with stock deduction on each add/remove
- Payment recording against AccAccount with balance update
- Sales returns (partial and full)
- Printable invoice view
- Invoice and quote number auto-generation

**Non-Goals:**
- GL double-entry journal posting (future Accounting module)
- Email invoice to customer (future enhancement)
- Multi-currency exchange rate calculations (currency stored on header, amounts in base currency for now)
- POS/thermal print format (can be added later)

## Decisions

### 1. Draft auto-created on form open
**Decision:** When the user opens the "New Invoice" form, a `POST /api/sales/invoices` with `Category=Draft` is immediately called, creating a stub record. All subsequent line item and header changes update this draft in place.

**Rationale:** Mirrors InventoryMSNV's `CreateDraftInvoice` exactly. Prevents data loss if the user navigates away mid-entry. The draft is soft-deleted if the user cancels without promoting.

### 2. Stock updated per line item operation, not at invoice save
**Decision:** `POST /api/sales/invoice-lines` (add line) immediately deducts stock; `DELETE /api/sales/invoice-lines/{id}` immediately restores it. Invoice promotion does not change stock.

**Rationale:** Mirrors InventoryMSNV's `CurrentItemsUpdate` call in `AddPaymentDetail` and `DeletePaymentDetail`. Stock is always current regardless of invoice status.

### 3. Payment recording triggers AccAccount update
**Decision:** `POST /api/sales/invoices/{id}/payments` saves a `SalesPaymentRecord` and calls the AccAccount service to credit the selected account. `DELETE` reverses it.

**Rationale:** Mirrors `UpdateAccAccountInPaymentModeHistory` / `UpdateAccAccountInDeletePaymentModeHistory` in InventoryMSNV. Keeps balance always current.

### 4. Invoice number sequences are per-category
**Decision:** Regular invoices use an "INV-" prefix with auto-increment; Draft uses "D-" prefix; Quote uses "QT-" prefix; Manual uses "M-" prefix. Sequences are derived from MAX(existing number) + 1 at generation time, not a separate sequence table.

**Rationale:** Matches InventoryMSNV's `GetInvoiceNo`/`GetQuoteNo` approach. Simple and sufficient at current scale.

### 5. SalesInvoiceLine stores product name snapshot
**Decision:** `SalesInvoiceLine.ItemName` stores the product name at time of line creation, not just a FK.

**Rationale:** If a product is renamed or soft-deleted after the invoice is saved, the historical invoice must still show the original name. Matches InventoryMSNV's `PaymentDetail.ItemName`.

## Risks / Trade-offs

- **[Risk] Draft invoices accumulate if users never promote or cancel** → Mitigation: Show draft invoices in a separate "Drafts" tab; add a bulk-delete-old-drafts admin action in a future cleanup pass.
- **[Risk] Stock race condition on concurrent line item adds** → Mitigation: Acceptable at current scale; add optimistic concurrency on `Product.StockQuantity` in a hardening pass.
- **[Risk] Complex frontend state** → The invoice editor manages header + N line items + M payment records simultaneously. Mitigation: Use TanStack Query invalidation on each mutation to keep the UI in sync with server state rather than managing local state manually.

## Migration Plan

1. Add `StockQuantity` column to existing `Product` table (default 0, nullable initially then set not-null)
2. Add 4 new tables: SalesInvoice, SalesInvoiceLine, SalesPaymentRecord, ReturnLog
3. Migration auto-runs on startup

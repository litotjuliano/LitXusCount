## Context

Purchasing is structurally identical to Sales but with the flow inverted: stock is added (not deducted) when purchase lines are saved, and payments debit the AccAccount (cash goes out to supplier, not in from customer). InventoryMSNV's `PurchasesPayment`/`PurchasesPaymentDetail` mirrors `Payment`/`PaymentDetail` exactly — same fields, same patterns.

The stock update service introduced in Phase 4 (`IStockService`) is reused directly — the only difference is the sign of the quantity change.

## Goals / Non-Goals

**Goals:**
- Full purchase invoice lifecycle mirroring Phase 4 Sales pattern
- Stock addition per line item (instead of deduction)
- Supplier payment recording with AccAccount debit
- Purchase returns (stock removed on return line)
- Separate purchase invoice number sequence

**Non-Goals:**
- Three-way match (PO vs receipt vs invoice) — too complex for initial phase
- Supplier credit terms / AP aging — future Accounting module
- Automatic reorder triggers — future enhancement

## Decisions

### 1. Reuse IStockService from Phase 4 with a sign parameter
**Decision:** The stock service called by Phase 4 line item operations takes an `isAddition: bool` parameter. Purchase line add passes `isAddition: true`; purchase line delete passes `isAddition: false`. Sales does the inverse.

**Rationale:** InventoryMSNV's `CurrentItemsUpdate` already has this `IsAddition` flag. One service, two callers, opposite signs.

### 2. Purchase payment debits AccAccount (not credits)
**Decision:** `POST /api/purchases/invoices/{id}/payments` calls AccAccount service with a Debit operation (money leaves the account to pay the supplier). Sales payments Credit the account (money comes in).

**Rationale:** Mirrors InventoryMSNV's `InvoicePaymentType.PurchasesInvoicePayment = 2` which triggers the same `UpdateAccAccountInPaymentModeHistory` but with reversed debit/credit direction.

### 3. No draft auto-creation for purchases
**Decision:** Purchase invoices do NOT auto-create a draft on form open — the user explicitly saves when ready. Only Sales has draft auto-creation.

**Rationale:** InventoryMSNV's purchasing form doesn't use the draft pattern. Purchase orders are typically more deliberate than sales invoices (no walk-in customer scenario).

## Risks / Trade-offs

- **[Risk] Code duplication with Sales** → Mitigation: Shared `IStockService` and `IAccAccountService` reduce the overlap to domain entity + DTO + controller, which is intentional separation.
- **[Risk] Purchase returns increasing stock when it should decrease** → Mitigation: Return lines set `IsReturn = true`; stock service checks this flag and inverts the sign again.

## Migration Plan

1. Add 3 new tables: PurchaseInvoice, PurchaseInvoiceLine, PurchasePaymentRecord
2. Migration auto-runs on startup
3. No seed data required

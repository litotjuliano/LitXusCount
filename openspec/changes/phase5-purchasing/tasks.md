# Phase 5: Purchasing — Implementation Tasks

## Section 1: Domain Entities

- [ ] 1.1 Create `PurchaseInvoice` entity — SupplierId FK, Category (int: 1=Regular/3=Quote), InvoiceNo, QuoteNo, SubTotal, DiscountAmount, VATAmount, GrandTotal, PaidAmount, DueAmount, PaymentStatusId, ReturnType, Notes, CurrencyId FK, audit fields, IsActive
- [ ] 1.2 Create `PurchaseInvoiceLine` entity — PurchaseInvoiceId FK, ProductId FK, ItemName (snapshot), Quantity, UnitPrice, ItemVAT, ItemVATAmount, ItemDiscount, ItemDiscountAmount, TotalAmount, IsReturn, audit fields
- [ ] 1.3 Create `PurchasePaymentRecord` entity — PurchaseInvoiceId FK, AccAccountId FK, ModeOfPayment, Amount, ReferenceNo, audit fields

## Section 2: Infrastructure — EF Core Configuration & Migration

- [ ] 2.1 Add EF Core configurations for 3 new entities
- [ ] 2.2 Register new DbSets in `ApplicationDbContext`
- [ ] 2.3 Add EF Core migration: `AddPurchaseInvoiceTables`
- [ ] 2.4 Add index on PurchaseInvoice.Category and PurchaseInvoice.CreatedAt

## Section 3: Application Layer — Purchase Invoice Service

- [ ] 3.1 Create `IPurchaseInvoiceService` with: `CreateAsync`, `UpdateHeaderAsync`, `GetPagedAsync(categoryFilter)`, `GetByIdAsync`, `DeleteAsync`
- [ ] 3.2 Create `IPurchaseInvoiceLineService` with: `AddLineAsync`, `RemoveLineAsync`, `GetByInvoiceAsync`
- [ ] 3.3 Create `IPurchasePaymentService` with: `RecordPaymentAsync`, `DeletePaymentAsync`, `GetByInvoiceAsync`
- [ ] 3.4 Define DTOs mirroring Phase 4 Sales DTOs with Supplier context
- [ ] 3.5 Implement `PurchaseInvoiceService` — assigns PO-/PQ-prefixed numbers from MAX+1; no draft auto-creation; `DeleteAsync` reverses all line stock additions and payment debits
- [ ] 3.6 Implement `PurchaseInvoiceLineService.AddLineAsync` — saves line, copies ItemName snapshot, calls `IStockService(isAddition: true)`, recalculates totals
- [ ] 3.7 Implement `PurchaseInvoiceLineService.RemoveLineAsync` — soft-deletes line, calls `IStockService(isAddition: false)`, recalculates totals; IsReturn=true lines invert the sign
- [ ] 3.8 Implement `PurchasePaymentService.RecordPaymentAsync` — saves payment, calls `IAccAccountService` to DEBIT account (money out to supplier), recalculates PaidAmount/DueAmount
- [ ] 3.9 Implement `PurchasePaymentService.DeletePaymentAsync` — soft-deletes payment, reverses AccAccount debit, recalculates totals

## Section 4: Permissions

- [ ] 4.1 Add `Purchasing` static class to `Permissions.cs` — View, Create, Edit, Delete, Manage sub-permissions

## Section 5: API Controllers

- [ ] 5.1 Create `PurchaseInvoicesController` at `src/LitXusCount.API/Controllers/Purchases/` — POST (create), PUT `/{id}` (header update), GET (paged with category filter), GET `/{id}`, DELETE `/{id}`
- [ ] 5.2 Create `PurchaseInvoiceLinesController` — POST (add line), DELETE `/{id}` (remove line)
- [ ] 5.3 Create `PurchasePaymentsController` — POST (record payment debit), DELETE `/{id}` (reverse debit), GET (by invoiceId)
- [ ] 5.4 Register all 3 services in `DependencyInjection.cs`

## Section 6: Frontend — API Client Layer

- [ ] 6.1 Create `Web/admin-dashboard/src/api/purchases/purchaseInvoices.ts`, `purchaseInvoiceLines.ts`, `purchasePayments.ts`

## Section 7: Frontend — Hooks

- [ ] 7.1 Create `usePurchaseInvoices.ts`, `usePurchaseInvoiceLines.ts`, `usePurchasePayments.ts`

## Section 8: Frontend — Pages & Components

- [ ] 8.1 Create `PurchaseInvoicesPage.tsx` — lists purchase invoices with category filter (Orders/Quotes), columns: InvoiceNo, Supplier, Date, GrandTotal, Status
- [ ] 8.2 Create `PurchaseInvoiceEditorPage.tsx` — header form (Supplier, Date, Notes) + line item table + payment recording section; no draft auto-creation; user explicitly creates

## Section 9: Frontend — Sidebar Navigation

- [ ] 9.1 Replace Purchasing placeholder with "Purchasing" group containing: Purchase Orders, Purchase Quotes

## Section 10: Verification

- [ ] 10.1 `dotnet build` passes
- [ ] 10.2 Create a purchase invoice — verify PO-prefixed number assigned
- [ ] 10.3 Add a purchase line — verify Product.StockQuantity INCREASED
- [ ] 10.4 Remove a purchase line — verify Product.StockQuantity DECREASED (opposite of Sales)
- [ ] 10.5 Record purchase payment — verify AccAccount.Balance DECREASED (debit, money out)
- [ ] 10.6 Delete purchase payment — verify AccAccount.Balance RESTORED
- [ ] 10.7 Delete purchase invoice — verify all stock additions and payment debits reversed
- [ ] 10.8 Frontend lists purchase orders and quotes separately

# Phase 4: Sales Invoices — Implementation Tasks

## Section 1: Domain Entities

- [x] 1.1 Add `StockQuantity` (int, default 0) column to existing `Product` entity
- [x] 1.2 Create `SalesInvoice` entity — CustomerId FK, Category (int: 1=Regular/2=Draft/3=Quote/4=Manual), InvoiceNo, QuoteNo, SubTotal, DiscountAmount, VATAmount, GrandTotal, PaidAmount, DueAmount, PaymentStatusId, ReturnType, PurchaseOrderNumber, Notes, CurrencyId FK, BranchId FK (nullable), audit fields, IsActive
- [x] 1.3 Create `SalesInvoiceLine` entity — SalesInvoiceId FK, ProductId FK, ItemName (snapshot), Quantity, UnitPrice, ItemVAT, ItemVATAmount, ItemDiscount, ItemDiscountAmount, TotalAmount, IsReturn, audit fields
- [x] 1.4 Create `SalesPaymentRecord` entity — SalesInvoiceId FK, AccAccountId FK, ModeOfPayment, Amount, ReferenceNo, audit fields
- [x] 1.5 Create `ReturnLog` entity — SalesInvoiceId FK, InvoiceNo, CustomerId FK, TranType, Note, audit fields

## Section 2: Infrastructure — EF Core Configuration & Migration

- [x] 2.1 Add EF Core configurations for all 4 new entities and updated Product entity
- [x] 2.2 Register new DbSets in `ApplicationDbContext`
- [x] 2.3 Add EF Core migration: `AddStockQuantityAndSalesInvoiceTables`
- [x] 2.4 Add index on SalesInvoice.Category and SalesInvoice.CreatedAt (for report queries in Phase 7)

## Section 3: Application Layer — Stock Service

- [x] 3.1 Create `IStockService` in Application layer with `UpdateStockAsync(productId, quantityChange, isAddition, actionType, sourceReference, notes)`
- [x] 3.2 Implement `StockService` — updates `Product.StockQuantity` atomically; writes `StockMovement` row (note: StockMovement table created in Phase 6, so Phase 4 implementation writes to it only after Phase 6 migration runs; use a feature-flag comment noting dependency)
- [x] 3.3 Register `IStockService` in `DependencyInjection.cs`

## Section 4: Application Layer — Sales Invoice Service

- [x] 4.1 Create `ISalesInvoiceService` with: `CreateDraftAsync`, `UpdateHeaderAsync`, `PromoteAsync(invoiceId, targetCategory)`, `GetPagedAsync(categoryFilter)`, `GetByIdAsync`, `DeleteAsync`
- [x] 4.2 Create `ISalesInvoiceLineService` with: `AddLineAsync`, `RemoveLineAsync`, `GetByInvoiceAsync`
- [x] 4.3 Create `ISalesPaymentService` with: `RecordPaymentAsync`, `DeletePaymentAsync`, `GetByInvoiceAsync`
- [x] 4.4 Define DTOs: `SalesInvoiceDto`, `SalesInvoiceCreateDto`, `SalesInvoiceHeaderUpdateDto`, `SalesInvoiceLineDto`, `SalesInvoiceLineCreateDto`, `SalesPaymentRecordDto`, `SalesPaymentRecordCreateDto`
- [x] 4.5 Implement `SalesInvoiceService` — `CreateDraftAsync` auto-generates D-prefixed number; `PromoteAsync` assigns INV-/QT-/M-prefixed number from MAX+1; `DeleteAsync` reverses all line stock + payment account movements
- [x] 4.6 Implement `SalesInvoiceLineService.AddLineAsync` — saves line, copies ItemName snapshot, calls `IStockService` to deduct, recalculates invoice totals
- [x] 4.7 Implement `SalesInvoiceLineService.RemoveLineAsync` — soft-deletes line, calls `IStockService` to restore stock, recalculates invoice totals
- [x] 4.8 Implement `SalesPaymentService.RecordPaymentAsync` — saves payment record, calls `IAccAccountService` to credit account, recalculates PaidAmount/DueAmount, sets PaymentStatusId=Paid if DueAmount=0
- [x] 4.9 Implement `SalesPaymentService.DeletePaymentAsync` — soft-deletes payment, reverses AccAccount credit, recalculates totals

## Section 5: Permissions

- [x] 5.1 Add `Sales` static class to `Permissions.cs` — View, Create, Edit, Delete, Manage (for payment recording and promotion) sub-permissions

## Section 6: API Controllers

- [x] 6.1 Create `SalesInvoicesController` at `src/LitXusCount.API/Controllers/Sales/` — POST (create draft), PUT `/{id}` (header update), PATCH `/{id}/promote`, GET (paged with category filter), GET `/{id}`, DELETE `/{id}`
- [x] 6.2 Create `SalesInvoiceLinesController` — POST (add line to invoice), DELETE `/{id}` (remove line)
- [x] 6.3 Create `SalesPaymentsController` — POST (record payment), DELETE `/{id}` (reverse payment), GET (by invoiceId)

## Section 7: Frontend — API Client Layer

- [x] 7.1 Create `Web/admin-dashboard/src/api/sales/salesInvoices.ts`
- [x] 7.2 Create `salesInvoiceLines.ts` and `salesPayments.ts`

## Section 8: Frontend — Hooks

- [x] 8.1 Create `useSalesInvoices.ts` — paged list hook with category filter, create draft, promote, delete
- [x] 8.2 Create `useSalesInvoiceLines.ts` — add/remove line hooks
- [x] 8.3 Create `useSalesPayments.ts` — record/delete payment hooks

## Section 9: Frontend — Pages & Components

- [x] 9.1 Create `SalesInvoicesPage.tsx` — lists invoices filtered by category (Invoices/Quotes/Drafts) with columns: InvoiceNo, Customer, Date, GrandTotal, Status; Create button opens new invoice form
- [x] 9.2 Create `SalesInvoiceEditorPage.tsx` — invoice header form (Customer, Date, Notes, PO number) + line item table (product dropdown, qty, price, discount, VAT) + payment recording section; auto-creates draft on mount; Save/Promote/Cancel actions
- [ ] 9.3 Create `SalesInvoiceDetailPage.tsx` — read-only view of a finalized invoice with print-friendly layout

## Section 10: Frontend — Sidebar Navigation

- [x] 10.1 Replace Sales placeholder with "Sales" group containing: Invoices (Regular/Manual), Quotes, Drafts — each passing a category filter to `SalesInvoicesPage`

## Section 11: Verification

- [x] 11.1 `dotnet build` passes
- [ ] 11.2 `StockQuantity` column added to Products table via migration
- [ ] 11.3 Open new invoice form — verify Draft auto-created in DB
- [ ] 11.4 Add a line item — verify Product.StockQuantity decremented
- [ ] 11.5 Remove a line item — verify Product.StockQuantity restored
- [ ] 11.6 Promote invoice to Regular — verify INV-prefixed number assigned
- [ ] 11.7 Record payment — verify AccAccount.Balance increased and DueAmount decreased
- [ ] 11.8 Delete payment — verify AccAccount.Balance reversed
- [ ] 11.9 Delete invoice — verify all stock and payment movements reversed
- [ ] 11.10 Frontend invoice list shows correct category filtering

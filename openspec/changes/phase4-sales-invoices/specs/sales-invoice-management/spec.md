## ADDED Requirements

### Requirement: Sales invoice header with four categories
The system SHALL support `SalesInvoice` records with a `Category` field: Regular (1), Draft (2), Quote (3), Manual (4). Each category has its own number sequence prefix. `InvoiceNo` is assigned on promotion to Regular/Manual; `QuoteNo` is assigned for Quote category; Draft uses a temporary "D-" prefixed number. Standard audit fields and `IsActive` soft-delete SHALL apply.

#### Scenario: Draft invoice is auto-created on new invoice form open
- **WHEN** a user opens the new invoice form
- **THEN** a POST to create a Draft SalesInvoice is made immediately, returning an Id the frontend uses for all subsequent line item and header operations

#### Scenario: Draft is promoted to Regular invoice
- **WHEN** a user saves a draft invoice as a Regular invoice
- **THEN** Category is set to 1, a new sequential InvoiceNo is assigned with the regular prefix, QuoteNo is cleared, and the invoice is persisted

#### Scenario: Invoice can be promoted to Quote
- **WHEN** a user saves as a Quote
- **THEN** Category is set to 3, a QuoteNo is assigned, and InvoiceNo is cleared

#### Scenario: Soft-deleting a draft cancels it
- **WHEN** a user cancels a Draft invoice
- **THEN** IsActive is set to false and the draft no longer appears in the invoice list

### Requirement: Sales invoice line items with real-time stock deduction
`SalesInvoiceLine` SHALL be added and removed individually via dedicated endpoints. Each add operation SHALL immediately deduct the line quantity from `Product.StockQuantity` and write a `StockMovement` row (ActionType=Sale). Each remove operation SHALL immediately restore the quantity. The invoice header totals (SubTotal, DiscountAmount, VATAmount, GrandTotal, DueAmount) SHALL be recalculated and updated on every line item change.

#### Scenario: Adding a line item deducts stock
- **WHEN** a user adds a line item for Product X with Quantity=5 to an open invoice
- **THEN** Product X StockQuantity decreases by 5, a StockMovement row is written, and the invoice totals are recalculated

#### Scenario: Removing a line item restores stock
- **WHEN** a user removes a line item for Product X with Quantity=5 from an open invoice
- **THEN** Product X StockQuantity increases by 5, a reversal StockMovement row is written, and invoice totals are recalculated

#### Scenario: Line item stores product name snapshot
- **WHEN** a line item is created
- **THEN** the current product name is copied into SalesInvoiceLine.ItemName and persisted independently of any future product name changes

### Requirement: Sales payment recording with AccAccount update
The system SHALL allow recording one or more payment entries against a SalesInvoice via `SalesPaymentRecord` (ModeOfPayment, AccAccountId, Amount, ReferenceNo). Each payment record saved SHALL credit the selected AccAccount and append an AccTransaction row. Deleting a payment record SHALL reverse the AccAccount credit. The invoice PaidAmount and DueAmount SHALL be recalculated after each payment change.

#### Scenario: Recording a payment credits the account
- **WHEN** a user records a cash payment of 500 against an invoice
- **THEN** the selected AccAccount.Balance increases by 500, an AccTransaction row is appended, and the invoice PaidAmount increases by 500

#### Scenario: Deleting a payment reverses the account credit
- **WHEN** a user deletes a previously recorded payment of 500
- **THEN** the AccAccount.Balance decreases by 500, a reversal AccTransaction row is appended, and invoice PaidAmount decreases by 500

#### Scenario: Invoice is marked Paid when DueAmount reaches zero
- **WHEN** total PaidAmount equals GrandTotal
- **THEN** invoice PaymentStatusId is set to Paid

### Requirement: Sales returns
A return SHALL be recorded by adding line items with `IsReturn = true` and negative quantity, setting `ReturnLog` entry and updating `ReturnType` on the invoice header (NoReturn=0, Partial=1, Full=2). Return lines SHALL restore stock (add back quantity).

#### Scenario: Return line restores stock
- **WHEN** a return line item is added for Product X with Quantity=2
- **THEN** Product X StockQuantity increases by 2

#### Scenario: Full return sets ReturnType to Full
- **WHEN** all original line quantities have corresponding return lines
- **THEN** SalesInvoice.ReturnType is set to 2 (Full)

### Requirement: Invoice number auto-generation
The system SHALL generate sequential invoice numbers per category: Regular = "INV-{n}", Draft = "D-{n}", Quote = "QT-{n}", Manual = "M-{n}" where n is MAX(existing same-category number) + 1.

#### Scenario: Each new regular invoice gets the next sequential number
- **WHEN** the last regular invoice number is "INV-42" and a new regular invoice is promoted
- **THEN** the new invoice is assigned "INV-43"

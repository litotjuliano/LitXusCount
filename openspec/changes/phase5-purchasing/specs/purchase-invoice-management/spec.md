## ADDED Requirements

### Requirement: Purchase invoice header
The system SHALL support `PurchaseInvoice` records linked to a `Supplier`. Fields mirror `SalesInvoice` except the customer FK is replaced by `SupplierId`. Category supports Regular (1) and Quote (3). Number sequences are separate from sales ("PO-{n}" for regular, "PQ-{n}" for quote). Standard audit fields and `IsActive` soft-delete apply. Unlike Sales, no Draft auto-creation occurs — the user explicitly saves.

#### Scenario: Purchase invoice is created
- **WHEN** an authorized user submits a new purchase invoice with a valid SupplierId and at least one line item
- **THEN** a PurchaseInvoice row is persisted with the assigned PO number and Category=Regular

#### Scenario: Purchase invoice can be created as a quote
- **WHEN** a user saves a purchase invoice as a Quote
- **THEN** Category is set to 3, a PQ-prefixed QuoteNo is assigned

#### Scenario: Soft-deleting a purchase invoice reverses stock and payments
- **WHEN** a user deletes a purchase invoice
- **THEN** all line item stock additions are reversed, all payment AccAccount debits are reversed, and IsActive is set to false

### Requirement: Purchase invoice line items with real-time stock addition
`PurchaseInvoiceLine` SHALL be added and removed via dedicated endpoints. Each add SHALL immediately add the line quantity to `Product.StockQuantity` and write a `StockMovement` row (ActionType=Purchase). Each remove SHALL immediately deduct the quantity. Invoice header totals SHALL be recalculated on every change.

#### Scenario: Adding a purchase line increases stock
- **WHEN** a user adds a line for Product X with Quantity=10 to a purchase invoice
- **THEN** Product X StockQuantity increases by 10 and a StockMovement row is written

#### Scenario: Removing a purchase line decreases stock
- **WHEN** a user removes a line for Product X with Quantity=10
- **THEN** Product X StockQuantity decreases by 10 and a reversal StockMovement row is written

### Requirement: Purchase payment recording with AccAccount debit
Recording a payment against a `PurchaseInvoice` via `PurchasePaymentRecord` SHALL debit the selected AccAccount (money paid to supplier) and append an AccTransaction row. Deleting a payment SHALL reverse the debit.

#### Scenario: Recording a purchase payment debits the account
- **WHEN** a user records a payment of 1000 against a purchase invoice
- **THEN** the selected AccAccount.Balance decreases by 1000 and an AccTransaction row is appended

#### Scenario: Deleting a purchase payment reverses the debit
- **WHEN** a user deletes a previously recorded purchase payment of 1000
- **THEN** the AccAccount.Balance increases by 1000 and a reversal AccTransaction row is appended

### Requirement: Purchase returns
Purchase return lines SHALL have `IsReturn = true`, which causes stock to be deducted (undoing the original addition). `ReturnType` on the header tracks NoReturn/Partial/Full status.

#### Scenario: Purchase return line deducts stock
- **WHEN** a return line for Product X with Quantity=3 is added to a purchase invoice
- **THEN** Product X StockQuantity decreases by 3

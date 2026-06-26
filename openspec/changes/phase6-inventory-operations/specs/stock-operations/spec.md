## ADDED Requirements

### Requirement: Unified StockMovement audit log
Every change to `Product.StockQuantity` — from Sales lines, Purchase lines, Adjustments, Transfers, and Damage write-offs — SHALL write a `StockMovement` row with: `ProductId`, `ActionType` (enum: Sale/Purchase/Adjustment/TransferOut/TransferIn/Damage), `QuantityBefore`, `QuantityChange`, `QuantityAfter`, `SourceReference` (e.g. "INV-001", "ADJ-001"), `Notes`, and standard audit fields. StockMovement rows are append-only and never edited or deleted.

#### Scenario: Every stock change writes a StockMovement row
- **WHEN** any operation changes Product.StockQuantity (sale line add, purchase line add, adjustment, transfer, damage)
- **THEN** a StockMovement row is written with the correct ActionType, before/after quantities, and a source reference

#### Scenario: StockMovement list is filterable by product
- **WHEN** a GET request is made to `/api/inventory/stock-movements?productId=5`
- **THEN** only StockMovement rows for Product 5 are returned, forming a complete stock card

### Requirement: Manual stock adjustment
The system SHALL allow recording a `StockAdjustment` (ProductId, AdjustmentQuantity — positive or negative, Reason, Date). Saving SHALL update `Product.StockQuantity` and write a StockMovement row (ActionType=Adjustment).

#### Scenario: Positive adjustment increases stock
- **WHEN** a user creates a stock adjustment for Product X with AdjustmentQuantity=+10
- **THEN** Product X StockQuantity increases by 10 and a StockMovement row is written

#### Scenario: Negative adjustment decreases stock
- **WHEN** a user creates a stock adjustment for Product X with AdjustmentQuantity=-3
- **THEN** Product X StockQuantity decreases by 3 and a StockMovement row is written

### Requirement: Inter-warehouse stock transfer
The system SHALL allow recording an `ItemTransferLog` (ProductId, FromWarehouseId, ToWarehouseId, Quantity, ReasonOfTransfer). Saving SHALL deduct quantity from the source context and add to the destination, writing two StockMovement rows (TransferOut and TransferIn), all committed atomically.

#### Scenario: Transfer moves stock between warehouses
- **WHEN** a user records a transfer of Product X, Quantity=5 from Warehouse A to Warehouse B
- **THEN** two StockMovement rows are written (TransferOut and TransferIn) and Product X StockQuantity net remains unchanged

#### Scenario: Transfer to same warehouse is rejected
- **WHEN** FromWarehouseId equals ToWarehouseId
- **THEN** the request is rejected with a validation error

### Requirement: Item request workflow
The system SHALL allow creating `ItemRequest` records (ProductId, RequestQuantity, FromWarehouseId, Status, Note). Status transitions: New(1) → Pending(2) → Sent(3) or Rejected(4) or ItemNotAvailable(5). Only forward transitions are permitted.

#### Scenario: Item request is created in New status
- **WHEN** a user creates an item request
- **THEN** a new ItemRequest row is persisted with Status=New

#### Scenario: Status can be advanced
- **WHEN** a user updates an item request Status from New to Pending
- **THEN** the Status is updated

#### Scenario: Invalid status transition is rejected
- **WHEN** a user attempts to set Status from Sent back to New
- **THEN** the request is rejected with a validation error

### Requirement: Damage item write-off
The system SHALL allow recording a `DamageItem` (ProductId, Quantity, ReasonOfDamage, Date). Saving SHALL deduct the quantity from `Product.StockQuantity` and write a StockMovement row (ActionType=Damage).

#### Scenario: Damage write-off reduces stock
- **WHEN** a user records a damage write-off for Product X with Quantity=2
- **THEN** Product X StockQuantity decreases by 2 and a StockMovement row with ActionType=Damage is written

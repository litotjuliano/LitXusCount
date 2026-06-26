# Phase 6: Inventory Operations — Implementation Tasks

## Section 1: Domain Entities

- [ ] 1.1 Create `Warehouse` entity — Code, Name, Address (nullable), IsActive, audit fields
- [ ] 1.2 Add `WarehouseId` (nullable FK) to existing `Product` entity
- [ ] 1.3 Create `StockMovement` entity — ProductId FK, ActionType (enum: Sale/Purchase/Adjustment/TransferOut/TransferIn/Damage), QuantityBefore, QuantityChange, QuantityAfter, SourceReference, Notes, audit fields (append-only, no IsActive needed)
- [ ] 1.4 Create `StockAdjustment` entity — ProductId FK, AdjustmentQuantity (signed int), Reason, AdjustmentDate, IsActive, audit fields
- [ ] 1.5 Create `ItemTransferLog` entity — ProductId FK, FromWarehouseId FK, ToWarehouseId FK, Quantity, ReasonOfTransfer, TransferDate, IsActive, audit fields
- [ ] 1.6 Create `ItemRequest` entity — ProductId FK, RequestQuantity, FromWarehouseId FK (nullable), Status (enum: New=1/Pending=2/Sent=3/Rejected=4/ItemNotAvailable=5), Note, IsActive, audit fields
- [ ] 1.7 Create `DamageItem` entity — ProductId FK, Quantity, ReasonOfDamage, DamageDate, IsActive, audit fields

## Section 2: Infrastructure — EF Core Configuration & Migration

- [ ] 2.1 Add `WarehouseConfiguration` — filtered unique index on Code where IsActive=true
- [ ] 2.2 Add configurations for StockMovement, StockAdjustment, ItemTransferLog, ItemRequest, DamageItem
- [ ] 2.3 Add Product FK config for WarehouseId
- [ ] 2.4 Register all new DbSets in `ApplicationDbContext`
- [ ] 2.5 Add EF Core migration: `AddInventoryOperationsTables`
- [ ] 2.6 Add index on StockMovement.ProductId and StockMovement.ActionType (for stock card queries)

## Section 3: Update Phase 4/5 Stock Service to Write StockMovement

- [ ] 3.1 Update `StockService.UpdateStockAsync` to also insert a `StockMovement` row with correct ActionType, QuantityBefore, QuantityChange, QuantityAfter, SourceReference
- [ ] 3.2 Verify existing sales line adds and purchase line adds now write StockMovement rows (manual integration test)

## Section 4: Application Layer — Inventory Operation Services

- [ ] 4.1 Create `IWarehouseService` with: `GetPagedAsync`, `GetAllActiveAsync`, `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`
- [ ] 4.2 Create `IStockAdjustmentService` with: `GetPagedAsync`, `GetByIdAsync`, `CreateAsync`
- [ ] 4.3 Create `IItemTransferService` with: `GetPagedAsync`, `GetByIdAsync`, `CreateAsync` (atomic deduct+add with two StockMovement rows)
- [ ] 4.4 Create `IItemRequestService` with: `GetPagedAsync`, `GetByIdAsync`, `CreateAsync`, `UpdateStatusAsync`
- [ ] 4.5 Create `IDamageItemService` with: `GetPagedAsync`, `GetByIdAsync`, `CreateAsync`
- [ ] 4.6 Define all DTOs
- [ ] 4.7 Implement `WarehouseService` — CRUD with soft-delete
- [ ] 4.8 Implement `StockAdjustmentService.CreateAsync` — saves record, calls `StockService` with signed qty, writes StockMovement (ActionType=Adjustment)
- [ ] 4.9 Implement `ItemTransferService.CreateAsync` — validates FromWarehouseId ≠ ToWarehouseId, deducts from source, adds to destination, commits two StockMovement rows atomically in one transaction
- [ ] 4.10 Implement `ItemRequestService.UpdateStatusAsync` — validates transition is forward-only; rejects backward transitions
- [ ] 4.11 Implement `DamageItemService.CreateAsync` — saves record, deducts from Product.StockQuantity, writes StockMovement (ActionType=Damage)

## Section 5: Permissions

- [ ] 5.1 Add `Inventory` static class to `Permissions.cs` — View, Create, Edit, Delete for each operation type

## Section 6: API Controllers

- [ ] 6.1 Create `WarehousesController` in `src/LitXusCount.API/Controllers/Settings/`
- [ ] 6.2 Create `StockMovementsController` in `Controllers/Inventory/` — GET (paged, filter by productId/actionType), GET `/{id}` (read-only, no create/edit/delete)
- [ ] 6.3 Create `StockAdjustmentsController` — GET (paged), GET `/{id}`, POST
- [ ] 6.4 Create `ItemTransfersController` — GET (paged), GET `/{id}`, POST
- [ ] 6.5 Create `ItemRequestsController` — GET (paged), GET `/{id}`, POST (create), PATCH `/{id}/status`
- [ ] 6.6 Create `DamageItemsController` — GET (paged), GET `/{id}`, POST
- [ ] 6.7 Register all services in `DependencyInjection.cs`

## Section 7: Frontend — API Client Layer

- [ ] 7.1 Create API client files: `warehouses.ts`, `stockMovements.ts`, `stockAdjustments.ts`, `itemTransfers.ts`, `itemRequests.ts`, `damageItems.ts`

## Section 8: Frontend — Hooks

- [ ] 8.1 Create hook files for each: `useWarehouses.ts`, `useStockMovements.ts`, `useStockAdjustments.ts`, `useItemTransfers.ts`, `useItemRequests.ts`, `useDamageItems.ts`

## Section 9: Frontend — Pages & Components

- [ ] 9.1 Add Warehouse to Settings sidebar and create `WarehousesPage.tsx` (Code/Name/Address columns, create/edit modal)
- [ ] 9.2 Create `StockMovementsPage.tsx` — read-only log table with ProductId filter, columns: Product/Date/ActionType/Before/Change/After/Reference
- [ ] 9.3 Create `StockAdjustmentsPage.tsx` — table with Product/Date/Qty/Reason columns; create form with product dropdown, signed quantity, reason, date
- [ ] 9.4 Create `ItemTransfersPage.tsx` — table with Product/From/To/Qty/Date columns; create form with two warehouse dropdowns + product + qty + reason
- [ ] 9.5 Create `ItemRequestsPage.tsx` — table with Product/Qty/Status/From columns; create form + status update action (dropdown of valid next statuses)
- [ ] 9.6 Create `DamageItemsPage.tsx` — table with Product/Qty/Date/Reason columns; create form

## Section 10: Frontend — Sidebar Navigation

- [ ] 10.1 Replace Inventory placeholder with "Inventory" group: Stock Movements, Stock Adjustments, Transfers, Item Requests, Damage Items

## Section 11: Verification

- [ ] 11.1 `dotnet build` passes
- [ ] 11.2 Migration creates all new tables including WarehouseId on Product
- [ ] 11.3 Create a warehouse, assign it to a product
- [ ] 11.4 Make a stock adjustment +10 — verify Product.StockQuantity increases and StockMovement row written
- [ ] 11.5 Record an item transfer — verify two StockMovement rows (TransferOut/TransferIn) written atomically
- [ ] 11.6 Verify transfer to same warehouse is rejected
- [ ] 11.7 Create item request, advance status New→Pending→Sent
- [ ] 11.8 Verify backward status transition (Sent→New) is rejected
- [ ] 11.9 Record a damage write-off — verify Product.StockQuantity decreases and StockMovement row written
- [ ] 11.10 Add a sales line and verify StockMovement row with ActionType=Sale is now written (retroactive Phase 4 service update)

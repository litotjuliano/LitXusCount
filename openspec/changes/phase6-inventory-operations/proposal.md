## Why

Stock adjustments, warehouse transfers, item requests, and damage write-offs happen outside of sales and purchasing. This phase migrates InventoryMSNV's standalone inventory operation entities (`ItemTransferLog`, `ItemRequest`, `DamageItemDeatils`) and introduces the `Warehouse` master data entity that enables multi-location stock tracking.

## What Changes

- Add `Warehouse` entity: named storage locations (master data)
- Add `Product.WarehouseId`: optional FK linking a product's primary warehouse
- Add `StockAdjustment`: manual quantity correction with reason; writes to `StockMovement` audit log
- Add `StockMovement` entity: immutable audit log of all quantity changes (replaces InventoryMSNV's `ItemsHistory`) — records old qty, new qty, transaction qty, action type, and source reference
- Add `ItemTransferLog`: move stock between warehouses; deducts from source, adds to destination
- Add `ItemRequest`: internal stock request workflow (New → Pending → Sent/Rejected)
- Add `DamageItem`: record damaged/written-off stock; deducts quantity
- Add frontend pages for all operations
- Add "Inventory" sidebar module (replaces placeholder)

## Capabilities

### New Capabilities

- `warehouse-management`: CRUD for warehouse locations; used as FK on products and transfer logs
- `stock-operations`: Stock adjustments, inter-warehouse transfers, item requests, and damage write-offs — all write to the StockMovement audit log

### Modified Capabilities

- `product-management`: Add `WarehouseId` (nullable FK to Warehouse) to `Product` entity
- `admin-dashboard-shell`: Replace Inventory placeholder with real Inventory Operations page

## Impact

- **Backend:** 5 new entities (Warehouse, StockAdjustment, StockMovement, ItemTransferLog, ItemRequest, DamageItem), shared stock movement logging service
- **Frontend:** Warehouse settings page, Stock Adjustment page, Transfer Log, Item Request page, Damage Items page
- **Database:** New migration; `Product` table gains `WarehouseId` column
- **Dependencies:** Requires Phase 2 (Product) and Phase 4/5 (StockMovement log shared with sales/purchasing stock updates)
- **Reference:** InventoryMSNV `ItemTransferLogController.cs`, `ItemRequestController.cs`, `DamageItemDetailsController.cs`, `ItemsHistoryController.cs`, `Common.cs` (`CurrentItemsUpdate`)

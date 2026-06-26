## Context

Phase 4 and 5 establish the stock quantity field on Product and the stock update service. Phase 6 adds the standalone operations that happen outside of Sales/Purchasing: manual adjustments, warehouse transfers, internal requests, and damage write-offs. All of these write to a unified `StockMovement` audit log (replacing InventoryMSNV's `ItemsHistory`).

## Goals / Non-Goals

**Goals:**
- Warehouse master data entity (prerequisite for transfers)
- Manual stock adjustments with reason
- Inter-warehouse stock transfers
- Internal item request workflow (New → Pending → Sent/Rejected)
- Damage item write-offs
- Unified StockMovement audit log for all quantity changes across all phases

**Non-Goals:**
- Barcode scanning / serial number tracking (InventoryMSNV has ItemSerialNumber — deferred)
- Batch/lot tracking — future enhancement
- Automated reorder points — future enhancement
- Multi-bin within a warehouse — future enhancement

## Decisions

### 1. StockMovement is the unified audit log for all stock changes
**Decision:** Every quantity change — from Sales lines, Purchase lines, Adjustments, Transfers, and Damage — writes a `StockMovement` row with `ActionType` (Sale/Purchase/Adjustment/Transfer/Damage), `QuantityBefore`, `QuantityAfter`, `QuantityChange`, `SourceReference` (e.g. "INV-001"), and `Notes`.

**Rationale:** InventoryMSNV's `ItemsHistory` only records changes from the common `CurrentItemsUpdate` service call — it misses transfers and damage. A unified log gives a complete stock card for any product. Phase 4/5 stock service is updated to write here too.

### 2. Warehouse is a new master data entity, not a system setting
**Decision:** `Warehouse` follows the same pattern as Category, UOM — Code, Name, IsActive, soft-delete. It gets its own settings page under System Settings (or a new Inventory Settings group).

**Rationale:** Warehouses are business-configurable, not system-level. Matches InventoryMSNV's `Warehouse` model.

### 3. ItemTransferLog deducts from source warehouse, adds to destination
**Decision:** A transfer atomically deducts from `FromWarehouseId` product stock and adds to `ToWarehouseId` product stock, writing two `StockMovement` rows, committed in a single transaction.

**Rationale:** Prevents stock from disappearing mid-transfer if the operation partially fails. Both movements or neither.

### 4. ItemRequest is a simple status-machine workflow
**Decision:** `ItemRequest` status transitions: New → Pending → Sent OR Rejected. No complex approval chain. Single endpoint `PATCH /api/inventory/item-requests/{id}/status` with the new status value.

**Rationale:** Matches InventoryMSNV's `RequestStatus` enum and the minimal workflow shown in the controller.

## Risks / Trade-offs

- **[Risk] Retroactively adding StockMovement rows for Phase 4/5 transactions** → Mitigation: Phase 4/5 stock service is updated in this phase to also write StockMovement rows; existing test data will not have historical movement rows but new transactions will.
- **[Risk] Product.WarehouseId is a single warehouse — products can't span warehouses** → Mitigation: This is a Phase 6 simplification. Multi-warehouse per product would require a product-warehouse quantity table, deferred to a future enhancement.

## Migration Plan

1. Add `Warehouse` table
2. Add `WarehouseId` column (nullable FK) to `Product` table
3. Add `StockMovement`, `StockAdjustment`, `ItemTransferLog`, `ItemRequest`, `DamageItem` tables
4. Update Phase 4/5 stock service to also write `StockMovement` rows going forward
5. Migration auto-runs on startup

## MODIFIED Requirements

### Requirement: Product CRUD with four GL account links and tiered pricing
The system SHALL provide CRUD management (list, create, edit, soft-delete) for `Product` entities. Each product SHALL carry: `Code` (unique among active products), `Code2` (optional alternate/barcode), `Description`, `CategoryId` (FK to Category), `UnitOfMeasureId` (FK to UnitOfMeasure), four nullable GL account FKs (`SalesRevenueGlAccountId`, `CogsGlAccountId`, `PurchaseApGlAccountId`, `PurchaseInventoryGlAccountId`), pricing fields (`CostPrice`, `PriceLevel1`, `PriceLevel2`, `PriceLevel3`, `MinQtyLevel2`, `MinQtyLevel3`), promo pricing fields (`PromoPrice`, `PromoStartDate`, `PromoEndDate`), `StockControl` (boolean), `StockQuantity` (integer, current quantity on hand — updated by sales/purchasing/inventory operations, not directly editable via this endpoint), and standard audit fields.

#### Scenario: Product is created with valid fields
- **WHEN** an authorized user submits a create request with a unique Code, Description, and valid CategoryId
- **THEN** a new Product row is persisted with `IsActive = true`, `StockQuantity = 0`, and audit fields populated

#### Scenario: StockQuantity is not directly editable via CRUD endpoint
- **WHEN** an authorized user submits a product edit including a StockQuantity value
- **THEN** the StockQuantity field is ignored — it is only changed by sales/purchasing/stock adjustment operations

#### Scenario: Duplicate Code among active products is rejected
- **WHEN** an authorized user attempts to create a Product with a Code matching an existing active product
- **THEN** the request is rejected with a validation error

#### Scenario: Soft delete sets IsActive to false
- **WHEN** an authorized user deletes an active Product
- **THEN** `IsActive` is set to false and the product no longer appears in list results

#### Scenario: List returns paged active products
- **WHEN** a list request is made with `page=1&pageSize=20`
- **THEN** only `IsActive = true` products are returned in a `PagedResult<T>` envelope

#### Scenario: List supports search by code or description
- **WHEN** a list request includes a `search` term
- **THEN** only products whose Code or Description contains the search term (case-insensitive) are returned

#### Scenario: All-active endpoint returns full active product list
- **WHEN** a GET request is made to `/api/settings/products/all-active`
- **THEN** the response is an array of all active Products including StockQuantity and pricing fields

## ADDED Requirements

### Requirement: Product CRUD with four GL account links and tiered pricing
The system SHALL provide CRUD management (list, create, edit, soft-delete) for `Product` entities. Each product SHALL carry: `Code` (unique among active products), `Code2` (optional alternate/barcode), `Description`, `CategoryId` (FK to Category), `UnitOfMeasureId` (FK to UnitOfMeasure), four nullable GL account FKs (`SalesRevenueGlAccountId`, `CogsGlAccountId`, `PurchaseApGlAccountId`, `PurchaseInventoryGlAccountId`), pricing fields (`CostPrice`, `PriceLevel1`, `PriceLevel2`, `PriceLevel3`, `MinQtyLevel2`, `MinQtyLevel3`), promo pricing fields (`PromoPrice`, `PromoStartDate`, `PromoEndDate`), `StockControl` (boolean), and standard audit fields. Listing SHALL follow the `PagedQuery`/`PagedResult<T>` contract.

#### Scenario: Product is created with valid fields
- **WHEN** an authorized user submits a create request with a unique Code, Description, and valid CategoryId
- **THEN** a new Product row is persisted with `IsActive = true` and audit fields populated

#### Scenario: Duplicate Code among active products is rejected
- **WHEN** an authorized user attempts to create a Product with a Code matching an existing active product
- **THEN** the request is rejected with a validation error

#### Scenario: Code can be reused after original is soft-deleted
- **WHEN** a Product with Code "PROD001" has been soft-deleted and a user creates a new product with Code "PROD001"
- **THEN** the creation succeeds

#### Scenario: Soft delete sets IsActive to false
- **WHEN** an authorized user deletes an active Product
- **THEN** `IsActive` is set to false and the product no longer appears in list results

#### Scenario: List returns paged active products
- **WHEN** a list request is made with `page=1&pageSize=20`
- **THEN** only `IsActive = true` products are returned in a `PagedResult<T>` envelope

#### Scenario: List supports search by code or description
- **WHEN** a list request includes a `search` term
- **THEN** only products whose Code or Description contains the search term (case-insensitive) are returned

### Requirement: Product all-active lookup endpoint
The system SHALL expose a `GET /api/settings/products/all-active` endpoint returning all `IsActive = true` Products (unpaged) for use as dropdown/lookup sources in Sales and Purchasing module forms.

#### Scenario: All-active endpoint returns full active product list
- **WHEN** a GET request is made to `/api/settings/products/all-active`
- **THEN** the response is an array of all active Products with at minimum Id, Code, Description, and pricing fields

### Requirement: Per-entity permissions on Product endpoints
Product endpoints SHALL require `Settings.Product.View` for reads and `Settings.Product.Create`/`Edit`/`Delete` for writes.

#### Scenario: Missing permission returns 403
- **WHEN** a user without `Settings.Product.View` attempts to list products
- **THEN** the response status is 403

#### Scenario: Holding the permission succeeds
- **WHEN** a user with `Settings.Product.Create` creates a product
- **THEN** the request succeeds

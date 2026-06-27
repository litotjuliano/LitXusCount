## 1. Domain Entities

- [x] 1.1 Add `GlAccount` entity (Code, Name, GlAccountType enum, ParentId self-ref, IsControl, OpeningBalance) extending AuditableEntity
- [x] 1.2 Add `Customer` entity (Code, Name, Name2, GlAccountId FK, billing address, consignee address, Phone, Email, ContactPerson, PaymentTermsDays, CreditLimit, IsLocked) extending AuditableEntity
- [x] 1.3 Add `Supplier` entity (Code, Name, GlAccountId FK, address fields, Phone, Email, ContactPerson, PaymentTermsDays, DefaultCurrencyId FK) extending AuditableEntity
- [x] 1.4 Add `Product` entity (Code, Code2, Description, CategoryId, UnitOfMeasureId, 4 GL account FKs, CostPrice, PriceLevel1/2/3, MinQtyLevel2/3, PromoPrice, PromoStartDate, PromoEndDate, StockControl) extending AuditableEntity

## 2. Permissions

- [x] 2.1 Add `Settings.GlAccount` permission class (View/Create/Edit/Delete) to `Permissions.cs`
- [x] 2.2 Add `Settings.Customer` permission class to `Permissions.cs`
- [x] 2.3 Add `Settings.Supplier` permission class to `Permissions.cs`
- [x] 2.4 Add `Settings.Product` permission class to `Permissions.cs`
- [x] 2.5 Mirror all 16 new permission strings in frontend `src/api/permissions.ts`

## 3. Application Layer (DTOs + Interfaces)

- [x] 3.1 Add `GlAccountDto` / `GlAccountUpsertDto` and `IGlAccountService` interface (ListAsync, ListAllActiveAsync, GetAsync, CreateAsync, EditAsync, DeleteAsync)
- [x] 3.2 Add `CustomerDto` / `CustomerUpsertDto` and `ICustomerService` interface
- [x] 3.3 Add `SupplierDto` / `SupplierUpsertDto` and `ISupplierService` interface
- [x] 3.4 Add `ProductDto` / `ProductUpsertDto` and `IProductService` interface

## 4. Infrastructure (EF Core + Services)

- [x] 4.1 Register `GlAccount`, `Customer`, `Supplier`, `Product` DbSets in `ApplicationDbContext`
- [x] 4.2 Configure EF Core: filtered unique indexes on `Code` (where `IsActive = true`), FK relationships with `DeleteBehavior.Restrict`, GlAccount self-reference
- [x] 4.3 Implement `GlAccountService` (circular parent check, referenced-entity check on delete)
- [x] 4.4 Implement `CustomerService`
- [x] 4.5 Implement `SupplierService`
- [x] 4.6 Implement `ProductService`
- [x] 4.7 Register all 4 services as Scoped in `DependencyInjection.cs`
- [x] 4.8 Add and apply EF Core migration for all 4 new tables

## 5. API Controllers

- [x] 5.1 Add `GlAccountsController` (`api/settings/gl-accounts`) — List, ListAllActive, Get, Create, Edit, Delete with permission policies
- [x] 5.2 Add `CustomersController` (`api/settings/customers`) — same pattern
- [x] 5.3 Add `SuppliersController` (`api/settings/suppliers`) — same pattern
- [x] 5.4 Add `ProductsController` (`api/settings/products`) — same pattern

## 6. Frontend API Client + Hooks

- [x] 6.1 Add `src/api/settings/glAccounts.ts` (GlAccountItem, GlAccountUpsert, AccountTypeLabels map, glAccountsApi)
- [x] 6.2 Add `src/api/settings/customers.ts` (CustomerItem, CustomerUpsert, customersApi)
- [x] 6.3 Add `src/api/settings/suppliers.ts` (SupplierItem, SupplierUpsert, suppliersApi)
- [x] 6.4 Add `src/api/settings/products.ts` (ProductItem, ProductUpsert, productsApi)
- [x] 6.5 Add `src/hook/useGlAccountSettings.ts` (paged query + allActive query + 3 mutations)
- [x] 6.6 Add `src/hook/useCustomerSettings.ts` (paged + glAccounts lookup + mutations)
- [x] 6.7 Add `src/hook/useSupplierSettings.ts` (paged + glAccounts + currencies lookups + mutations)
- [x] 6.8 Add `src/hook/useProductSettings.ts` (paged + glAccounts + categories + UOM + suppliers lookups + mutations)

## 7. Frontend Pages + Components

- [x] 7.1 Add `src/components/settings/GlAccountSettingsLayer.tsx` (form: Code, Name, Type select, Parent dropdown, IsControl checkbox, OpeningBalance; table: Code, Name, Type, Parent, Opening Bal)
- [x] 7.2 Add `src/components/settings/CustomerSettingsLayer.tsx` (sectioned form with billing address, consignee address, terms; table: Code, Name, GL Account, Phone, Email, Locked badge)
- [x] 7.3 Add `src/components/settings/SupplierSettingsLayer.tsx` (form with GL Account and Currency dropdowns; table: Code, Name, GL Account, Phone, Email)
- [x] 7.4 Add `src/components/settings/ProductSettingsLayer.tsx` (sectioned form: General, GL Accounts, Pricing, Promotion, Stock Control; table: Code, Description, Category, Cost Price, Selling Price)
- [x] 7.5 Add `src/pages/GlAccountsPage.tsx`
- [x] 7.6 Add `src/pages/CustomersPage.tsx`
- [x] 7.7 Add `src/pages/SuppliersPage.tsx`
- [x] 7.8 Add `src/pages/ProductsPage.tsx`

## 8. Routing + Navigation

- [x] 8.1 Add routes in `App.tsx` for `/settings/gl-accounts`, `/settings/customers`, `/settings/suppliers`, `/settings/products`
- [x] 8.2 Add "Master Data" sidebar group in `MasterLayout.tsx` with permission-gated entries for all 4 entities

## 9. Verification

- [x] 9.1 Confirm `dotnet build` passes with no errors
- [ ] 9.2 Confirm EF Core migration applies cleanly against a fresh database
- [ ] 9.3 Test GL Account CRUD via Swagger — create, list, edit, soft-delete, verify duplicate Code rejected
- [ ] 9.4 Test Customer CRUD via Swagger — verify GL Account FK validated, Code uniqueness enforced
- [ ] 9.5 Test Supplier CRUD via Swagger — verify Currency FK optional, GL Account required
- [ ] 9.6 Test Product CRUD via Swagger — verify all 4 GL account FK fields accepted as nullable
- [ ] 9.7 Verify frontend pages load and CRUD operations work end-to-end for all 4 entities
- [ ] 9.8 Verify Master Data sidebar group shows/hides correctly based on permissions

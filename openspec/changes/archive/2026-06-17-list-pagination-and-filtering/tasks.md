## 1. Shared backend contract

- [x] 1.1 Add `PagedQuery` class to `LitXusCount.Application.Common` (`Page = 1`, `PageSize = 20`, `Search`, `SortBy`, `SortDescending`)
- [x] 1.2 Add `PagedResult<T>` record to `LitXusCount.Application.Common` (`Items`, `TotalCount`, `Page`, `PageSize`)
- [x] 1.3 Clamp `PageSize` to a maximum of 100 in one shared place (e.g. a `PagedQuery.EffectivePageSize` property) so every consumer gets the cap for free

## 2. Backend: simple lookups (shared base)

- [x] 2.1 Change `ILookupService`/`ILookupService`-derived interfaces' list method to `Task<PagedResult<LookupItemDto>> ListAsync(PagedQuery query, CancellationToken ct = default)`
- [x] 2.2 Implement paging/search/sort in `LookupServiceBase<TEntity>`: search against `Name`/`Description`, sortable columns whitelist (`name`, `description`), default sort `Name` ascending
- [x] 2.3 Add a small unpaged `ListAllActiveAsync()` method (capped at 100) to the same base, for dropdown/reference consumers that need a plain list (e.g. Company Info's default-pickers)
- [x] 2.4 Update the 5 simple lookup controllers (`PaymentTypesController`, etc., via `LookupControllerBase<TService>`) to bind `[FromQuery] PagedQuery query` and call `ListAsync(query)`

## 3. Backend: Currency, VatPercentage, EmailConfig

- [x] 3.1 Add `ListAsync(PagedQuery query)` to `ICurrencyService`/`CurrencyService` — searchable by `Name`/`Code`/`Country`, sortable by `name`, `code`
- [x] 3.2 Add `ListAsync(PagedQuery query)` to `IVatPercentageService`/`VatPercentageService` — searchable by `Name`, sortable by `name`, `percentage`
- [x] 3.3 Add `ListAsync(PagedQuery query)` to `IEmailConfigService`/`EmailConfigService` — searchable by `Email`/`Hostname`, sortable by `email`, `hostname`
- [x] 3.4 Add the same unpaged `ListAllActiveAsync()` escape hatch to each of these three, for Company Info's default-pickers
- [x] 3.5 Update `CurrenciesController`, `VatPercentagesController`, `EmailConfigsController` to bind `[FromQuery] PagedQuery query` and call `ListAsync(query)`

## 4. Backend verification

- [x] 4.1 `dotnet build` succeeds with zero errors across all 4 projects
- [x] 4.2 Seed more than one page of a lookup entity (or temporarily lower page size) and confirm `GET .../currencies?page=2&pageSize=1` returns the second row with correct `totalCount`
- [x] 4.3 Confirm `search` narrows results and `totalCount` reflects the narrowed set
- [x] 4.4 Confirm sorting by a whitelisted column changes row order, and an unrecognized `sortBy` value falls back to the default sort without erroring
- [x] 4.5 Confirm `pageSize=1000` is clamped to 100 rows max

## 5. Frontend: shared pattern

- [x] 5.1 Add `usePaginatedQuery` hook: owns `page`/`pageSize`/`search`/`sortBy`/`sortDescending` state, debounces `search` (350ms), builds a TanStack Query `queryKey`/`queryFn` from a passed-in paged-list API function
- [x] 5.2 Add `PaginatedTable` component: renders sortable column headers (click toggles `sortBy`/`sortDescending`), a search input wired to the hook, and page controls (prev/next, page number, page-size select)
- [x] 5.3 Update `lookupApi.ts`/`createLookupApi` and the Currency/VatPercentage/EmailConfig API modules with a `list(query: PagedQuery)` function returning `PagedResult<T>`, plus keep/add a `listAllActive()` function for dropdown consumers
- [x] 5.4 Update `useLookupSettings` and the three custom hooks (`useCurrencySettings`, `useVatPercentageSettings`, `useEmailConfigSettings`) to expose both the new paged query (via `usePaginatedQuery`) and the existing mutations

## 6. Frontend: rewire the 8 System Settings list pages

- [x] 6.1 Rewire `LookupSettingsLayer` (covers 5 entities) to render its table via `PaginatedTable`/`usePaginatedQuery` instead of rendering the full unpaged array
- [x] 6.2 Rewire `CurrencySettingsLayer` to the same pattern
- [x] 6.3 Rewire `VatPercentageSettingsLayer` to the same pattern
- [x] 6.4 Rewire `EmailConfigSettingsLayer` to the same pattern
- [x] 6.5 Update `CompanyInfoSettingsLayer`'s Currency/VatPercentage/EmailConfig dropdown population to use the new `listAllActive()` functions instead of the now-paged `list()`

## 7. End-to-end verification

- [x] 7.1 Log in, open a lookup list page, and confirm search/sort/page controls are visible and functional in the browser
- [x] 7.2 Type into the search box and confirm the list narrows after a brief pause (not on every keystroke)
- [x] 7.3 Click a sortable column header and confirm row order changes; click again and confirm it reverses
- [x] 7.4 Confirm Company Info's default Currency/VAT/Email Config dropdowns still list all active options (unaffected by paging)

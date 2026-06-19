## Context

`system-lookup-management`'s 8 list endpoints currently call `ListActiveAsync()` with no parameters and return every active row; the frontend renders the full array in a plain HTML table. This was an accepted gap for the initial System Settings change (small reference tables), but the user has asked for paging/sorting/filtering now, applied as the standard pattern for all future modules — not a one-off fix scoped to System Settings.

## Goals / Non-Goals

**Goals:**
- One shared backend contract (`PagedQuery` request shape, `PagedResult<T>` response envelope) used by every list endpoint, starting with the 8 System Settings lookups.
- One shared frontend pattern (a `PaginatedTable` component + a `usePaginatedQuery` hook) used by every list page, starting with the 8 System Settings pages.
- Server-side paging, sorting, and search — not client-side — so the pattern actually scales to large tables in later modules (Inventory items, Sales transactions).

**Non-Goals:**
- No change to `CompanyInfo` — it's a singleton with no list view, exempt from this pattern.
- No client-side library (e.g. `datatables.net`, already bundled in WowDash) — the existing System Settings pages are plain Bootstrap tables with custom inline forms, and pulling in a jQuery-based grid now would conflict with that pattern and with the foundation's stated risk of jQuery/React conflicts. A hand-rolled component keeps everything in React's control.
- No full-text/fuzzy search — `search` is a simple case-insensitive "contains" match against name/description (or entity-specific fields), consistent with the simplicity of the data being searched.
- No saved views, column visibility toggles, or export — out of scope for this pass.

## Decisions

1. **`PagedQuery` and `PagedResult<T>` live in `LitXusCount.Application.Common`, next to `ServiceResult<T>`.** Same rationale as `ServiceResult<T>`: a small, shared data shape, not a generic repository — every list-returning Application service method takes a `PagedQuery` and returns `Task<PagedResult<TDto>>`. Alternative considered: a different shape per module — rejected, since the entire point is one contract every module reuses.

2. **`PagedQuery` is a plain class with default-valued properties (`Page = 1`, `PageSize = 20`, `Search = null`, `SortBy = null`, `SortDescending = false`), bound via `[FromQuery]` on each controller action.** A record with a positional constructor was considered, but a class with settable properties binds from query strings with zero ambiguity and no constructor-matching edge cases.

3. **`PageSize` is clamped server-side to a max of 100** (silently capped, not rejected) — prevents a client from requesting the entire table in one page and defeating the purpose, without needing a hard validation error for an edge case that doesn't matter to the caller.

4. **`SortBy` is validated against a per-entity whitelist of column names (e.g. `"name"`, `"description"` for the simple lookups; `"name"`, `"code"` for Currency), falling back to a default sort (`Name` ascending) for any unrecognized value** — never interpolated directly into a query expression, to avoid letting a client sort by an arbitrary/unexposed column.

5. **`Search` matches name/description (or the entity's equivalent fields) via `EF.Functions.Like` with wrapped wildcards, applied before counting and before paging**, so `TotalCount` reflects the filtered set, not the whole table — required for page-count math to make sense on the frontend.

6. **Frontend: one `PaginatedTable` presentational component (renders headers, sort indicators, rows via a render-prop/children pattern, and pagination controls) plus one `usePaginatedQuery` hook (owns `page`/`pageSize`/`search`/`sortBy`/`sortDescending` state, debounces `search` by 350ms, and feeds a TanStack Query `useQuery` whose `queryKey` includes all five params)** so every list page wires the same few pieces together instead of each page reimplementing fetch-and-render state. Alternative considered: bake paging directly into each entity's existing hook (`useCurrencySettings`, etc.) — rejected, since that would require five near-identical rewrites instead of one shared hook reused five times, mirroring the same DRY reasoning already used for `LookupServiceBase`/`createLookupApi`.

7. **The 5 simple lookups continue to share one generic implementation; Currency/VatPercentage/EmailConfig get their own paged query implementations**, mirroring the existing split between `LookupServiceBase<TEntity>` and the three custom services — paging logic added to the existing shared base, and duplicated (not abstracted further) into the three custom services, consistent with the project's existing line between shared-because-identical and separate-because-different.

## Risks / Trade-offs

- **[Risk] Existing frontend API modules (`currenciesApi.list()` etc.) currently return a bare array; callers outside list pages (e.g. `CompanyInfoSettingsLayer`'s default-currency/VAT/email dropdowns) use `useCurrencySettings().listQuery.data` expecting an array.** → Mitigation: keep a *second*, unpaged `listAllActive()` method on each API module/hook (small, capped query asking for `pageSize=100`, sufficient for dropdown population) separate from the new paged `list(query)` method, so dropdown consumers don't need to unwrap a `PagedResult` just to populate a `<select>`.
- **[Risk] Breaking change to existing list endpoint response shape** (`LookupItemDto[]` → `PagedResult<LookupItemDto>`) **could break any external caller.** → Mitigation: none needed — the only caller is this same admin dashboard, updated in the same change; no external API consumers exist yet.
- **[Trade-off] Hand-rolled pagination UI instead of `datatables.net`** — more code to write now, but avoids the jQuery/React friction flagged as a risk in the foundation change, and keeps the pattern consistent with the rest of System Settings' plain-React forms.

## Migration Plan

No data migration — this only changes API request/response shapes and frontend components, no schema change. Existing seeded data is unaffected; the seeder (`SystemSettingsSeeder`) continues to work unchanged since it talks to `ApplicationDbContext` directly, not through the now-paged service methods.

## Open Questions

None — query/response shapes and the search/sort whitelist approach are concrete enough to implement directly.

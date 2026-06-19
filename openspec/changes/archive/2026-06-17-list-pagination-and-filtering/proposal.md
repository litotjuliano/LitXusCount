## Why

The System Settings list endpoints and pages (`system-lookup-management`) currently return every active row in one response with no search, sort, or page-size control — fine for the handful of rows seeded today, but it does not scale, and every future module (Inventory, Sales, Purchasing, Finance) will need the same list-browsing behavior. Without a standard pattern established now, each module would invent its own paging/sorting/filtering shape, producing inconsistent APIs and duplicated frontend code.

## What Changes

- Add a generic `PagedQuery` (page, page size, search term, sort field, sort direction) and `PagedResult<T>` (items, total count, page, page size) shape in `LitXusCount.Application.Common`, to be the standard contract for every list endpoint going forward — not specific to System Settings.
- Change all 8 System Settings lookup list endpoints (`GET /api/settings/{resource}`) to accept `page`, `pageSize`, `search`, `sortBy`, `sortDescending` query parameters and return a `PagedResult<T>` envelope instead of a bare array.
- Add a reusable frontend `PaginatedTable` component plus a `usePaginatedQuery` hook pattern (search input with debounce, sortable column headers, page-size selector, page controls) in the admin dashboard, and rewire all 8 System Settings list pages to use it.
- `CompanyInfo` is unaffected — it is a singleton with no list view.

## Capabilities

### Modified Capabilities
- `clean-architecture-backend`: adds the standard paginated/sortable/filterable list endpoint contract (`PagedQuery`/`PagedResult<T>`) that all current and future list endpoints SHALL follow.
- `admin-dashboard-shell`: adds the standard reusable paginated/sortable/filterable list/table UI pattern that all current and future list pages SHALL follow.
- `system-lookup-management`: the 8 lookup list endpoints and pages adopt the new paging/sorting/filtering contract in place of the previous "return everything" behavior.

## Impact

- Backend: `LitXusCount.Application.Common` (new shared types), all `Settings/*Service` implementations and their interfaces, all `Settings/*Controller` list actions.
- Frontend: new `PaginatedTable` component and `usePaginatedQuery` hook, all 8 System Settings list pages and their API/hook modules.
- Establishes the pattern every later business module's list views (Inventory, Sales, Purchasing, Finance) are expected to reuse rather than reinvent.

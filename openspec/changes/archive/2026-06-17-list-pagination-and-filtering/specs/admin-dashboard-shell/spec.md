## ADDED Requirements

### Requirement: Reusable paginated/sortable/filterable list pattern
The admin dashboard SHALL use one reusable `PaginatedTable` component and one reusable `usePaginatedQuery` hook for every list page, rather than each page implementing its own fetch/search/sort/page state. The hook SHALL own `page`, `pageSize`, `search`, `sortBy`, and `sortDescending` state, debounce `search` input, and include all five as part of the TanStack Query cache key so each combination is cached independently.

#### Scenario: List page renders search, sortable columns, and page controls
- **WHEN** a list page built on `PaginatedTable`/`usePaginatedQuery` is rendered
- **THEN** it shows a search input, clickable/sortable column headers, and page navigation controls (current page, total pages, page-size selector)

#### Scenario: Search input is debounced
- **WHEN** a user types into the search input
- **THEN** the underlying API request is not sent on every keystroke, but after a short pause in typing (debounced)

#### Scenario: Changing page, sort, or search refetches the list
- **WHEN** a user changes the page number, clicks a sortable column header, or changes the search term
- **THEN** the list refetches from the API with the updated `page`/`sortBy`/`sortDescending`/`search` parameters and re-renders the new result set

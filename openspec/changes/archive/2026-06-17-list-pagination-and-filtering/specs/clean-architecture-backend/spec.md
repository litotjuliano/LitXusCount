## ADDED Requirements

### Requirement: Standard paginated/sortable/filterable list endpoint contract
Every API endpoint that returns a list of rows SHALL accept `page`, `pageSize`, `search`, `sortBy`, and `sortDescending` query parameters and return a `PagedResult<T>` envelope (`items`, `totalCount`, `page`, `pageSize`) — defined once in `LitXusCount.Application.Common` and reused by every module, not reimplemented per module. `pageSize` SHALL be clamped server-side to a maximum of 100. `sortBy` SHALL be validated against a per-entity whitelist of sortable columns, falling back to a default sort if the value is unrecognized. `search` SHALL be applied (as a case-insensitive "contains" match against the entity's name/description or equivalent fields) before counting and before paging, so `totalCount` reflects the filtered set.

#### Scenario: List endpoint returns a paged envelope
- **WHEN** a GET request is made to a list endpoint with `page=2&pageSize=10`
- **THEN** the response body is a `PagedResult<T>` object containing up to 10 items, the requested `page` and `pageSize`, and a `totalCount` reflecting the full filtered row count

#### Scenario: Search filters before counting
- **WHEN** a GET request is made to a list endpoint with a `search` value matching a subset of rows
- **THEN** `totalCount` reflects only the matching rows, and `items` contains only matching rows for the requested page

#### Scenario: Unrecognized sortBy falls back to default
- **WHEN** a GET request is made to a list endpoint with a `sortBy` value that is not in that entity's whitelist of sortable columns
- **THEN** the endpoint does not error, and instead sorts by its default column

#### Scenario: pageSize is capped
- **WHEN** a GET request is made to a list endpoint with `pageSize=1000`
- **THEN** the server clamps the effective page size to 100 rather than returning the full table or rejecting the request

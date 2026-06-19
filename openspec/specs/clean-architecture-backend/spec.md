## Purpose

Defines the 4-project Clean Architecture backend solution structure (Domain/Application/Infrastructure/API), its layering rules, persistence approach, and cross-cutting API concerns (health, Swagger, CORS) that every backend module builds on.

## Requirements

### Requirement: Layered project structure
The solution SHALL consist of four backend projects — `LitXusCount.Domain`, `LitXusCount.Application`, `LitXusCount.Infrastructure`, `LitXusCount.API` — each targeting `net10.0`, with project references forming a strict one-directional dependency graph: Domain has no project references; Application references Domain only; Infrastructure references Application and Domain; API references Application and Infrastructure.

#### Scenario: Domain has no outward dependencies
- **WHEN** `LitXusCount.Domain.csproj` is inspected
- **THEN** it has zero `<ProjectReference>` entries and zero third-party NuGet package references (POCO entities only)

#### Scenario: Build fails if layering is violated
- **WHEN** a project reference is added from `LitXusCount.Domain` to `LitXusCount.Infrastructure` (or any other backward reference)
- **THEN** `dotnet build` fails with a circular or invalid reference error

### Requirement: Persistence via EF Core against PostgreSQL only
The `LitXusCount.Infrastructure` project SHALL contain `ApplicationDbContext` and all EF Core configuration, targeting PostgreSQL exclusively (no SQL Server/MySQL provider).

#### Scenario: Database provider is PostgreSQL
- **WHEN** the API starts up and resolves `ApplicationDbContext`
- **THEN** it connects using `Npgsql.EntityFrameworkCore.PostgreSQL` against the configured PostgreSQL connection string

#### Scenario: Filtered unique indexes use PostgreSQL filter syntax
- **WHEN** a lookup entity's filtered unique index (active-rows-only uniqueness, per `system-lookup-management`) is created by a migration
- **THEN** the generated filter predicate uses PostgreSQL syntax (double-quoted identifiers, boolean literals), not SQL Server's bracket-quoted/integer-literal syntax

### Requirement: Repositories never leak IQueryable
Repository interfaces defined in `LitXusCount.Application` SHALL expose methods that return materialized results (concrete lists/DTOs and counts), never `IQueryable<T>`. No generic `IRepository<T>` SHALL be introduced.

#### Scenario: Repository method signature returns materialized data
- **WHEN** a repository interface method is declared for a query operation (e.g. a paged/filtered list)
- **THEN** its return type is a concrete collection type (e.g. `Task<(IReadOnlyList<T>, int TotalCount)>`), not `IQueryable<T>`

### Requirement: API exposes health and API documentation endpoints
The API project SHALL expose a `/health` endpoint returning HTTP 200 when the application and database connection are healthy, and SHALL serve Swagger/OpenAPI documentation.

#### Scenario: Health check succeeds
- **WHEN** a GET request is made to `/health` while the API and database are running
- **THEN** the response status is 200

#### Scenario: Swagger UI is reachable
- **WHEN** a GET request is made to the Swagger UI route in the Development environment
- **THEN** the page loads and lists the API's available endpoints

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

### Requirement: CORS allows the frontend origin
The API SHALL be configured with a CORS policy permitting requests from the admin dashboard frontend's configured origin(s) (dev and prod).

#### Scenario: Frontend dev server can call the API
- **WHEN** the admin dashboard dev server (running on its configured origin) makes a request to the API
- **THEN** the response includes CORS headers permitting that origin, and the browser does not block the request

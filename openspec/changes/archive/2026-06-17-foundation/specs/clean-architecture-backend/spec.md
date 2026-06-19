## ADDED Requirements

### Requirement: Layered project structure
The solution SHALL consist of four backend projects — `LitXusCount.Domain`, `LitXusCount.Application`, `LitXusCount.Infrastructure`, `LitXusCount.API` — each targeting `net10.0`, with project references forming a strict one-directional dependency graph: Domain has no project references; Application references Domain only; Infrastructure references Application and Domain; API references Application and Infrastructure.

#### Scenario: Domain has no outward dependencies
- **WHEN** `LitXusCount.Domain.csproj` is inspected
- **THEN** it has zero `<ProjectReference>` entries and zero third-party NuGet package references (POCO entities only)

#### Scenario: Build fails if layering is violated
- **WHEN** a project reference is added from `LitXusCount.Domain` to `LitXusCount.Infrastructure` (or any other backward reference)
- **THEN** `dotnet build` fails with a circular or invalid reference error

### Requirement: Persistence via EF Core against SQL Server only
The `LitXusCount.Infrastructure` project SHALL contain `ApplicationDbContext` and all EF Core configuration, targeting SQL Server exclusively (no MySQL/PostgreSQL provider).

#### Scenario: Database provider is SQL Server
- **WHEN** the API starts up and resolves `ApplicationDbContext`
- **THEN** it connects using `Microsoft.EntityFrameworkCore.SqlServer` against the configured SQL Server connection string

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

### Requirement: CORS allows the frontend origin
The API SHALL be configured with a CORS policy permitting requests from the admin dashboard frontend's configured origin(s) (dev and prod).

#### Scenario: Frontend dev server can call the API
- **WHEN** the admin dashboard dev server (running on its configured origin) makes a request to the API
- **THEN** the response includes CORS headers permitting that origin, and the browser does not block the request

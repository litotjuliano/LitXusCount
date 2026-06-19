## MODIFIED Requirements

### Requirement: Persistence via EF Core against PostgreSQL only
The `LitXusCount.Infrastructure` project SHALL contain `ApplicationDbContext` and all EF Core configuration, targeting PostgreSQL exclusively (no SQL Server/MySQL provider).

#### Scenario: Database provider is PostgreSQL
- **WHEN** the API starts up and resolves `ApplicationDbContext`
- **THEN** it connects using `Npgsql.EntityFrameworkCore.PostgreSQL` against the configured PostgreSQL connection string

#### Scenario: Filtered unique indexes use PostgreSQL filter syntax
- **WHEN** a lookup entity's filtered unique index (active-rows-only uniqueness, per `system-lookup-management`) is created by a migration
- **THEN** the generated filter predicate uses PostgreSQL syntax (double-quoted identifiers, boolean literals), not SQL Server's bracket-quoted/integer-literal syntax

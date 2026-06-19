## Why

LitXusCount currently runs on SQL Server, a deliberate choice made in the foundation change. The user now wants to switch to PostgreSQL, driven by hosting cost — SQL Server licensing/hosting is materially more expensive than Postgres on most cloud platforms, and this project is still pre-production with no data worth preserving through an in-place migration.

## What Changes

- Replace the SQL Server EF Core provider (`Microsoft.EntityFrameworkCore.SqlServer`) with the PostgreSQL provider (`Npgsql.EntityFrameworkCore.PostgreSQL`) in `LitXusCount.Infrastructure`.
- Remove the unused `AspNetCore.HealthChecks.SqlServer` package from `LitXusCount.API` (confirmed dead — the actual health check call, `AddDbContextCheck<ApplicationDbContext>()`, is already provider-agnostic).
- Fix the one piece of hand-written, provider-specific raw SQL in the model: 6 filtered unique index definitions (`Currency`, `PaymentType`, `PaymentStatus`, `CustomerType`, `Category`, `UnitOfMeasure`) use SQL Server's bracket-quoted filter syntax (`"[IsActive] = 1"`), which is invalid PostgreSQL syntax (`"\"IsActive\" = true"` is the equivalent).
- Delete the existing SQL-Server-generated migrations and regenerate a single fresh `InitialCreate` migration against the PostgreSQL provider — there is no production data to carry forward, so regenerating from the current model is correct rather than attempting to convert SQL-Server-specific migration SQL.
- Update the connection string format in `appsettings.json` from SQL Server's to PostgreSQL's.

## Capabilities

### Modified Capabilities
- `clean-architecture-backend`: the "Persistence via EF Core against SQL Server only" requirement changes to PostgreSQL as the sole supported provider (still a single-provider stance, just a different provider — not reopening the foundation's "no multi-database support" decision).

## Impact

- Backend: `LitXusCount.Infrastructure.csproj`, `LitXusCount.API.csproj`, `DependencyInjection.cs`, `ApplicationDbContext.cs`, `appsettings.json`, and the entire `Persistence/Migrations/` folder are affected.
- No frontend changes — the API surface (routes, DTOs, behavior) is unaffected; this is purely a backend persistence-provider swap.
- Requires PostgreSQL to be installed and reachable locally before migrations can be applied and the change verified end-to-end.

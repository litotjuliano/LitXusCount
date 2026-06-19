## Context

The foundation change's design.md picked SQL Server deliberately, citing a real dependency-version conflict InventoryMSNV hit when supporting SQL Server + MySQL + PostgreSQL simultaneously, and the absence of any concrete near-term need for a different provider. That calculus has changed: hosting cost is now a concrete, named driver, and the project remains pre-production (foundation, System Settings, pagination, and Users/Roles/Permissions were all built this same session) — there is no production data that an in-place migration would need to preserve.

Codebase research (this change) found exactly one place with hand-written, provider-specific raw SQL: `ApplicationDbContext.OnModelCreating` has 6 `.HasFilter("[IsActive] = 1")` calls (filtered unique indexes on `Currency`, `PaymentType`, `PaymentStatus`, `CustomerType`, `Category`, `UnitOfMeasure`) using SQL Server's bracket-quoted-identifier-plus-integer-boolean syntax. Everything else in the model (column types, identity/serial columns, the 6 `EF.Functions.Like` search call sites) is generated or translated automatically per-provider by EF Core and required no source changes.

## Goals / Non-Goals

**Goals:**
- Swap the EF Core provider end-to-end (packages, registration, connection string, migrations) with zero behavior change visible to API consumers (same routes, same DTOs, same validation rules).
- Fix the filtered-index syntax so the existing "unique among active rows" behavior (established in `system-lookup-management`) continues to work identically under PostgreSQL.

**Non-Goals:**
- No multi-provider support — this is a straight swap (SQL Server → PostgreSQL), not adding PostgreSQL as a second option. The foundation's "no multi-database support" reasoning (the InventoryMSNV dependency-conflict precedent) still applies; only the chosen single provider changes.
- No production data migration tooling — there is no production data yet. If this ever needs to run again against a real SQL Server database with real data, that would be a separate, much more careful change (export/import or a dual-write cutover), not this one.
- No change to hosting/deployment configuration beyond the connection string shape — actually provisioning a production PostgreSQL instance is out of scope for this change, which only touches the local dev environment and the codebase.

## Decisions

1. **Regenerate migrations from scratch rather than attempting to convert the existing SQL-Server-generated ones.** EF Core migrations contain provider-specific generated SQL (`SqlServerModelBuilderExtensions.UseIdentityColumns`, SQL Server type names like `nvarchar(max)`, `datetime2`); there is no supported "convert this migration to a different provider" path, and hand-editing generated migration code is exactly the kind of fragile, easy-to-get-subtly-wrong work that regenerating from the (already correct, provider-agnostic) C# entity model avoids entirely. Alternative considered: edit the existing migration files in place — rejected as needless risk for a greenfield database with nothing to preserve.

2. **Fix the filtered-index syntax directly in `ApplicationDbContext`, not via a provider-conditional branch.** Since this change drops SQL Server support entirely (Decision/Non-Goal: no multi-provider), the fix is a straight replacement of the filter string, not an `if (Database.IsNpgsql())`-style branch that would only make sense if both providers had to coexist.

3. **`AspNetCore.HealthChecks.SqlServer` is removed, not replaced with a Postgres-specific health-check package.** The actual health check registered in `Program.cs` is `AddHealthChecks().AddDbContextCheck<ApplicationDbContext>()` — a provider-agnostic EF Core health check that calls `CanConnectAsync()` regardless of provider. The `AspNetCore.HealthChecks.SqlServer` package was present in the `.csproj` but never actually invoked in code, so there's nothing to replace.

## Risks / Trade-offs

- **[Risk] PostgreSQL must be installed and reachable locally before this change can be verified end-to-end (migrations applied, API started, login/CRUD tested).** → Mitigation: the user is installing it natively; code/config changes (provider swap, filter fix, migration regeneration against a model, not a live database for the `migrations add` step) can proceed first, with database-dependent steps (`migrations add` actually needs SOME reachable Postgres instance to connect to, even an empty one, to run — see Open Question) paused until confirmed reachable.
- **[Trade-off] One-time loss of local dev/seed data** (the SQL Server database's existing rows) — acceptable, since `SystemSettingsSeeder` and `Program.cs`'s dev-user seeding already regenerate all of this automatically on first run against a fresh database; nothing manual needs to be re-entered.

## Migration Plan

1. Swap packages and provider registration; fix the 6 `HasFilter` calls.
2. Delete `Persistence/Migrations/` entirely.
3. With a reachable (even empty) local PostgreSQL instance, run `dotnet ef migrations add InitialCreate` to scaffold one fresh migration from the current model.
4. Run `dotnet ef database update` to create the schema.
5. Start the API — `Program.cs`'s existing dev-seed block (roles, dev users, permission claims, `SystemSettingsSeeder`) repopulates all reference/dev data automatically, exactly as it does today against SQL Server.

## Open Questions

- `dotnet ef migrations add` needs to connect to a real PostgreSQL server to introspect provider capabilities even when scaffolding a brand-new migration (EF Core does not require the target database/schema to already exist, but it does need the *server* to be reachable). This means step 3 above can't run until the user's local PostgreSQL installation is up — confirmed as the "open item before implementation starts" in the approved plan.

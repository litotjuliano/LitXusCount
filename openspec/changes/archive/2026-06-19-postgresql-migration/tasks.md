## 1. Package and provider swap

- [x] 1.1 Remove `Microsoft.EntityFrameworkCore.SqlServer` from `LitXusCount.Infrastructure.csproj`, add `Npgsql.EntityFrameworkCore.PostgreSQL` (check NuGet for the version compatible with EF Core 10.0.9)
- [x] 1.2 Remove `AspNetCore.HealthChecks.SqlServer` from `LitXusCount.API.csproj` (confirmed unused in code)
- [x] 1.3 Update `DependencyInjection.cs`: `options.UseSqlServer(...)` → `options.UseNpgsql(...)`

## 2. Model fix

- [x] 2.1 Replace all 6 `.HasFilter("[IsActive] = 1")` calls in `ApplicationDbContext.OnModelCreating` (Currency, PaymentType, PaymentStatus, CustomerType, Category, UnitOfMeasure) with PostgreSQL syntax: `.HasFilter("\"IsActive\" = true")`

## 3. Connection string

- [x] 3.1 Replace the SQL Server connection string in `appsettings.json` with a PostgreSQL connection string, using real credentials once the user confirms their local PostgreSQL installation details

## 4. Migrations

- [x] 4.1 Delete `src/LitXusCount.Infrastructure/Persistence/Migrations/` entirely
- [x] 4.2 With a reachable local PostgreSQL server, run `dotnet ef migrations add InitialCreate` to scaffold one fresh migration from the current model
- [x] 4.3 Run `dotnet ef database update` to create the schema

## 5. Verification

- [x] 5.1 `dotnet build` succeeds with zero errors across all 4 projects
- [x] 5.2 Start the API, confirm `/health` returns 200 against PostgreSQL
- [x] 5.3 Confirm the dev-seed block repopulates roles, dev users, permission claims, and `SystemSettingsSeeder`'s reference data on first run
- [x] 5.4 Log in as the seeded `SuperAdmin` user and confirm the JWT contains the expected permission claims (same check used in `user-roles-permissions`)
- [x] 5.5 Exercise CRUD + the filtered-uniqueness behavior on at least one lookup entity (e.g. create a Currency, confirm duplicate-code rejection, soft-delete, confirm the code can be reused) — proves the PostgreSQL filtered index fix actually works
- [x] 5.6 Quick browser pass: log in via the admin dashboard, confirm System Settings and Users/Roles pages still load and function against the new database

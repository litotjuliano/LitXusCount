## Why

LitXusCount has only ever run locally. The user wants to try deploying it to DigitalOcean to see the app running on real infrastructure, continuing the same cost-conscious direction that drove the PostgreSQL switch. The project also wasn't a git repository at all until this change — there was no version control, let alone a path to a hosting platform that deploys from a Git remote.

## What Changes

- Initialize git for the repository (it had none) and push to a new GitHub repo (`https://github.com/litotjuliano/LitXusCount`), since DigitalOcean App Platform deploys from a connected Git host.
- Add a multi-stage `Dockerfile` for `LitXusCount.API` (build context = repo root, since the API depends on the other 3 projects via project references).
- Add a DigitalOcean App Platform spec (`.do/app.yaml`) defining one app with three components sharing a single domain: the API (Dockerfile-based service, routed at `/api`), the admin dashboard (static site, routed at `/`), and a PostgreSQL "Dev Database". Single-domain path-based routing avoids both CORS configuration and the chicken-and-egg problem of the frontend needing the backend's URL before it exists.
- `Program.cs`: run `ApplicationDbContext.Database.MigrateAsync()` unconditionally on startup (any environment) — App Platform has no separate migration-job step in this setup, so the API applies its own pending migrations on boot.
- `Program.cs`: the existing dev-seed block (roles, `SuperAdmin`/`Admin` dev accounts with a hardcoded password, `SystemSettingsSeeder` reference data) is now gated by `IsDevelopment() OR SEED_DEMO_DATA=true`, instead of always running whenever `ASPNETCORE_ENVIRONMENT=Development`. This makes seeding hardcoded demo credentials into a deployed environment a deliberate, visible opt-in via an App Platform env var, not an accidental side effect of how the environment happens to be named.
- Frontend: add `Web/admin-dashboard/.env.production` with an empty `VITE_API_BASE_URL`, so production builds make relative requests (works because of the same-domain path routing above).

## Capabilities

### New Capabilities
- `digitalocean-deployment`: the Dockerfile, the App Platform spec (service/static-site/database topology, single-domain routing), the auto-migrate-on-startup behavior, and the `SEED_DEMO_DATA` opt-in seeding gate.

## Impact

- New files: `src/LitXusCount.API/Dockerfile`, `.do/app.yaml`, `.gitignore`, `Web/admin-dashboard/.env.production`.
- Modified: `src/LitXusCount.API/Program.cs` (migration + seed-gating behavior change — affects local dev too, see design.md).
- The repository itself goes from no version control to a GitHub-hosted git repo for the first time.
- This change does not yet complete a live deployment — see tasks.md for what's done (repo prep) versus what's still pending (push, DigitalOcean App Platform console setup, first live deploy, verification).

## Context

The user just subscribed to DigitalOcean and wants to try deploying LitXusCount, with no strong opinion on the specific approach beyond "I just subscribed and wanted to try." Neither PostgreSQL nor Docker nor any DO tooling (`doctl`) was installed locally when this change started; the user opted to install PostgreSQL natively for local dev (covered by the prior `postgresql-migration` change) and confirmed PostgreSQL is now reachable. The repository had no git history at all — this change is also the project's first commit.

## Goals / Non-Goals

**Goals:**
- Get LitXusCount running on DigitalOcean with the least possible ceremony, appropriate for someone trying the platform out rather than standing up a production system.
- Reuse the existing local-dev architecture (4-project Clean Architecture backend, Vite-built React frontend, PostgreSQL) without forking it into a separate "deployment" variant.
- Keep secrets (JWT signing key, DB password) out of the git repository.

**Non-Goals:**
- No production-grade reliability (the App Platform "Dev Database" tier has no automated backups/failover — fine for trying things out, not for real data). Upgrading to a Managed Database is a future, separate decision once/if this becomes more than a trial.
- No custom domain/DNS — the user has no domain yet; this change only targets the `*.ondigitalocean.app` URL App Platform assigns automatically.
- No CI/CD pipeline beyond App Platform's own "redeploy on push to main" default behavior — no GitHub Actions, no separate staging environment.
- No Droplet/self-managed-VM path — explicitly ruled out in favor of App Platform for lower operational overhead while trying the platform out.

## Decisions

1. **App Platform over a Droplet.** Confirmed directly: the user wants the lowest-friction way to try DigitalOcean, not to learn server administration. App Platform builds and runs the Dockerfile, terminates HTTPS, and assigns a working URL with no manual nginx/systemd/certbot setup — the trade-off (less control, somewhat higher cost per resource than a bare Droplet) is acceptable for a trial.

2. **One App Platform app, three components, single shared domain with path-based routing** (`api` service at `/api`, `web` static site at `/`, `db` Postgres dev database) — rather than deploying the API and frontend as separate apps with separate `*.ondigitalocean.app` URLs. Single-domain routing eliminates two problems at once: (a) CORS — same-origin requests need no CORS policy at all; (b) the bootstrap ordering problem where the frontend's build needs to bake in the backend's URL, but the backend's URL isn't known until after it's deployed. With path routing, the frontend's `VITE_API_BASE_URL` is simply empty (relative requests), valid from the very first deploy. Alternative considered: deploy backend first, note its URL, then build frontend with that URL baked in, then update backend CORS to allow the frontend's URL — rejected as a more error-prone, multi-pass sequence for someone trying the platform for the first time.

3. **`preserve_path_prefix: true` on the API's route.** Without this, App Platform strips the matched prefix (`/api`) before forwarding to the component, but the API's own controllers already expect routes starting with `api/...` (e.g. `[Route("api/auth")]`). Preserving the prefix means the path arrives at the backend unchanged, matching what already works locally.

4. **App Platform's "Dev Database" tier, not a standalone Managed Database cluster.** Confirmed via Non-Goals: this is a trial, not production. The dev database tier is bundled into the same app spec (simpler — one thing to create, not two) and is the cheapest Postgres option App Platform offers. Revisit if/when this needs real reliability guarantees.

5. **Database migrations run automatically on API startup** (`Database.MigrateAsync()` unconditionally, not gated by environment), rather than a separate App Platform "Job" component or a manual `dotnet ef database update` step. Alternative considered: a pre-deploy Job component running EF migrations — rejected as more moving parts (a second Dockerfile-driven component, or a `run_command` override) for a single-instance trial deployment where the multi-instance race-condition risk of auto-migrating on every boot doesn't apply yet (only `instance_count: 1`).

6. **Demo/seed data (`SuperAdmin`/`Admin` accounts with a hardcoded password, System Settings reference data) is gated by a new `SEED_DEMO_DATA` env var, defaulting to unset/false outside Development**, rather than continuing to gate it purely on `ASPNETCORE_ENVIRONMENT=Development`. Setting `ASPNETCORE_ENVIRONMENT=Production` (as the deployed app does) would otherwise silently skip seeding — and with no users in the database at all, nobody could ever log in to the freshly deployed app. The explicit `SEED_DEMO_DATA=true` env var in `.do/app.yaml` makes "this deployment has hardcoded demo credentials" a visible, deliberate, and easily-reversible choice (flip the env var to `false` and redeploy) rather than an accidental side effect of an environment name.

7. **Secrets are typed directly into App Platform's encrypted env var fields, never committed.** `.do/app.yaml` ships with an obvious placeholder for `Jwt__SigningKey` (`REPLACE_BEFORE_DEPLOY_...`) and constructs `ConnectionStrings__DefaultConnection` from the database component's auto-injected binding variables (`${db.HOSTNAME}`, `${db.PORT}`, etc.) rather than a literal connection string — consistent with how local dev already keeps the Postgres password in `dotnet user-secrets` instead of `appsettings.json`.

8. **GitHub as the Git host, not DigitalOcean Container Registry.** App Platform supports deploying from a pushed Docker image, but that requires building the image locally first — and Docker isn't installed on this machine. Deploying from a GitHub repo lets App Platform build the Dockerfile itself server-side, which is also the standard, best-documented App Platform path.

## Risks / Trade-offs

- **[Risk] `SEED_DEMO_DATA=true` ships a publicly-known hardcoded password (`DevPassw0rd!`) on a real, internet-reachable deployment.** → Mitigation: explicitly called out to the user as something to flip off (`SEED_DEMO_DATA=false`, redeploy) once they've logged in once and created real accounts. Acceptable for the trial window, not acceptable to leave on indefinitely.
- **[Risk] Auto-migrating on every API boot is unsafe with more than one instance** (concurrent migration races). → Mitigation: `instance_count: 1` in the app spec; revisit (move to a pre-deploy Job) before ever scaling horizontally.
- **[Trade-off] App Platform's Dev Database has no backups.** Acceptable per Non-Goals — this is explicitly not where production data should live yet.
- **[Risk] This is the project's first-ever git commit — `git add -A` stages ~677 files in one shot.** → Mitigation: reviewed `git status --short` before committing to confirm no `bin/`, `obj/`, `node_modules/`, or `dist/` directories were accidentally staged (the `.gitignore` added in this same change covers them).

## Migration Plan

No data migration — this is the first deployment, nothing to move. Sequence: commit → push to GitHub → create the App Platform app from `.do/app.yaml` → replace the `Jwt__SigningKey` placeholder with a real secret in the console → first deploy → verify `/health`, login, and a few pages → decide whether to flip `SEED_DEMO_DATA` off.

## Open Questions

- None blocking — the remaining work (push, App Platform console setup, first deploy, verification) is sequential execution, not unresolved design.

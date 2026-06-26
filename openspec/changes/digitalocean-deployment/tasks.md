## 1. Repository preparation

- [x] 1.1 Add `.gitignore` covering `bin/`, `obj/`, `node_modules/`, `dist/`, IDE/OS noise, and local secret files
- [x] 1.2 Add `src/LitXusCount.API/Dockerfile` (multi-stage .NET 10 build, repo-root build context, listens on port 8080)
- [x] 1.3 Add `.do/app.yaml` (API service routed at `/api`, static site routed at `/`, Postgres dev database)
- [x] 1.4 Add `Web/admin-dashboard/.env.production` with empty `VITE_API_BASE_URL`
- [x] 1.5 `Program.cs`: run `Database.MigrateAsync()` unconditionally on startup
- [x] 1.6 `Program.cs`: gate demo/seed data behind `IsDevelopment() OR SEED_DEMO_DATA=true` instead of `IsDevelopment()` alone
- [x] 1.7 Confirm `dotnet build` still succeeds after the `Program.cs` changes

## 2. Git and GitHub

- [x] 2.1 `git init`, set default branch to `main` (repository had no prior git history)
- [x] 2.2 `git add -A` and verify via `git status --short` that no `bin/`, `obj/`, `node_modules/`, or `dist/` directories were accidentally staged
- [x] 2.3 Create the first commit
- [x] 2.4 Add the GitHub remote (`https://github.com/litotjuliano/LitXusCount`) and push `main`

## 3. DigitalOcean App Platform setup

- [ ] 3.1 In the DigitalOcean console, create a new App Platform app from the GitHub repo, using `.do/app.yaml` as the starting spec (or paste it in during creation)
- [ ] 3.2 Replace the `Jwt__SigningKey` placeholder with a real random secret (32+ chars) directly in the App Platform console's encrypted env var field
- [ ] 3.3 Confirm the `db` component (Postgres dev database) is created and bound to the `api` service
- [ ] 3.4 Trigger the first deploy and watch the build logs for both the `api` (Docker) and `web` (static site) components

## 4. Verification

- [ ] 4.1 Confirm the app's assigned `*.ondigitalocean.app` URL loads the sign-in page
- [ ] 4.2 Confirm `/health` (via the same domain, `/api`-routed) returns 200
- [ ] 4.3 Log in with the seeded `SuperAdmin` dev account (requires `SEED_DEMO_DATA=true`) and confirm the dashboard loads
- [ ] 4.4 Spot-check a System Settings page and the Users/Roles pages against the deployed, PostgreSQL-backed API
- [ ] 4.5 Decide whether to set `SEED_DEMO_DATA=false` and redeploy once real accounts exist (or leave it on while still actively trying things out — note this explicitly with the user, don't silently decide)

## ADDED Requirements

### Requirement: Containerized API deployable via Dockerfile
The `LitXusCount.API` project SHALL be buildable into a container image via a Dockerfile whose build context is the repository root (required because the API depends on the Domain/Application/Infrastructure projects via project references). The resulting image SHALL listen on port 8080.

#### Scenario: API container builds and starts
- **WHEN** the Dockerfile is built with the repository root as context and run
- **THEN** the API starts and listens on port 8080, with `/health` reachable

### Requirement: Single-domain path-based routing for API and frontend
The API and the admin dashboard SHALL be deployed as components of one application sharing a single domain, with the API routed at the `/api` path prefix (prefix preserved when forwarded) and the frontend routed at `/`. The frontend's production build SHALL make relative API requests (no absolute base URL), relying on this shared-domain routing rather than CORS.

#### Scenario: Frontend reaches the API without CORS configuration
- **WHEN** the deployed frontend makes a request to a relative path under `/api`
- **THEN** the request reaches the API component on the same domain and succeeds without any CORS preflight/policy being involved

### Requirement: Database migrations apply automatically on API startup
The API SHALL apply any pending Entity Framework Core migrations automatically when it starts, in every environment, rather than requiring a manual or separate migration step.

#### Scenario: Fresh deployment creates its own schema
- **WHEN** the API starts against a database with no schema yet
- **THEN** it applies all migrations before serving requests, and the database has the expected tables afterward

### Requirement: Demo/seed data requires explicit opt-in outside Development
Seeding demo data (dev role/user accounts with a known password, and System Settings reference data) SHALL run when the environment is Development, OR when an explicit `SEED_DEMO_DATA` flag is set to true — and SHALL NOT run in any other case, so that a deployed environment never seeds hardcoded credentials unless explicitly requested.

#### Scenario: Production environment does not seed by default
- **WHEN** the API starts with `ASPNETCORE_ENVIRONMENT=Production` and `SEED_DEMO_DATA` unset
- **THEN** no demo roles, users, or reference data are created

#### Scenario: Production environment seeds when explicitly requested
- **WHEN** the API starts with `ASPNETCORE_ENVIRONMENT=Production` and `SEED_DEMO_DATA=true`
- **THEN** demo roles, users, and reference data are created exactly as they are in Development

### Requirement: Secrets are never committed to the repository
The JWT signing key and the database connection string (including its password) SHALL be supplied via deployment-platform-managed secret/environment variable storage, never as literal values in any file tracked by git.

#### Scenario: App spec contains no real secret values
- **WHEN** `.do/app.yaml` is inspected
- **THEN** the JWT signing key field is an obvious placeholder requiring manual replacement, and the database connection string is constructed from the platform's auto-injected database binding variables rather than a literal password

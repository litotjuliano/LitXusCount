## Why

LitXusCount is a brand-new Clean Architecture ERP solution (reference: InventoryMSNV, a legacy ASP.NET Core MVC inventory/accounting app, used only as a functional/business-logic reference). Before any business module (Inventory, Sales, etc.) can be built, the project needs a working foundation: a properly layered backend (Domain/Application/Infrastructure/API), real authentication (ASP.NET Core Identity + JWT), and a frontend admin shell (WowDash, licensed via Envato Elements) wired to that authentication. Without this, every later module would be built on an unverified, unintegrated stack.

## What Changes

- Scaffold a 4-project Clean Architecture backend solution: `LitXusCount.Domain`, `LitXusCount.Application`, `LitXusCount.Infrastructure`, `LitXusCount.API`, with strict one-directional dependencies (Domain has none; Application depends on Domain; Infrastructure depends on Application+Domain; API depends on Application+Infrastructure).
- Wire ASP.NET Core Identity + JWT bearer authentication into the API project, backed by EF Core against SQL Server (no multi-DB support, unlike InventoryMSNV — out of scope for now).
- Configure CORS, Swagger/OpenAPI, and a `/health` endpoint on the API.
- Retrofit TypeScript onto the WowDash admin dashboard template (currently plain JavaScript despite its Envato listing claiming TypeScript support), strip ~150 unrelated bundled demo pages down to the navigation shell, login screen, and reusable table/form primitives, and add placeholder nav entries for the planned business modules (Inventory, Sales, Purchasing, Finance, Identity/Admin).
- Wire the WowDash `SignInPage` to the real backend JWT login endpoint, replacing any mock/static auth, with token storage and route protection.

## Capabilities

### New Capabilities
- `clean-architecture-backend`: The 4-project solution structure and project-reference rules (Domain/Application/Infrastructure/API), EF Core + `ApplicationDbContext` setup, CORS, Swagger, and the `/health` endpoint.
- `identity-jwt-auth`: ASP.NET Core Identity user/role model and JWT bearer issuance/validation, modeled on InventoryMSNV's password policy, lockout, and refresh-token patterns but adapted for an API-only (no cookie/Razor login) flow.
- `admin-dashboard-shell`: The stripped-down, TypeScript-converted WowDash frontend shell — layout, routing, navigation placeholders per business module — with no business data yet.
- `frontend-auth-integration`: The login flow connecting the dashboard shell to the real Identity/JWT API, including token storage, attaching the token to subsequent API calls, and route protection for unauthenticated users.

### Modified Capabilities
(none — this is a greenfield project, nothing exists yet)

## Impact

- New solution and 4 new backend projects under `C:\LitXus Systems\LitXusCount\src\`.
- Existing WowDash frontend at `C:\LitXus Systems\LitXusCount\Web\admin-dashboard` is significantly modified: TypeScript tooling added, ~150 demo files removed, login page rewired.
- No existing code is being replaced or migrated (InventoryMSNV is a separate, untouched reference project) — this is purely additive within LitXusCount.
- Establishes the database (SQL Server, via EF Core migrations) and the JWT auth contract that every later business module (Inventory, Sales, Purchasing, Finance) will depend on.

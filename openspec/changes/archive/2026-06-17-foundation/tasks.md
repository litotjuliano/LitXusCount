## 1. Backend solution scaffold

- [x] 1.1 Create solution file (`LitXusCount.sln` or `.slnx`) at `C:\LitXus Systems\LitXusCount`
- [x] 1.2 Create `LitXusCount.Domain` class library (net10.0), zero package references
- [x] 1.3 Create `LitXusCount.Application` class library (net10.0), project reference to Domain only
- [x] 1.4 Create `LitXusCount.Infrastructure` class library (net10.0), project references to Application + Domain; add EF Core + SQL Server + Identity EF Core packages
- [x] 1.5 Create `LitXusCount.API` Web API project (net10.0), project references to Application + Infrastructure
- [x] 1.6 Add all four projects to the solution and verify `dotnet build` succeeds with no source files yet

## 2. Identity + JWT auth

- [x] 2.1 Add `ApplicationUser : IdentityUser` to `LitXusCount.Infrastructure/Identity/` (not Domain — `IdentityUser` requires the Identity package, which would violate Domain's zero-package-reference rule)
- [x] 2.2 Add `ApplicationDbContext : IdentityDbContext<ApplicationUser>` to `LitXusCount.Infrastructure/Persistence/`
- [x] 2.3 Configure EF Core SQL Server provider registration in `LitXusCount.API/Program.cs`
- [x] 2.4 Create initial EF Core migration (Identity tables) and apply it to a local SQL Server database
- [x] 2.5 Configure ASP.NET Core Identity (password policy, lockout) in `Program.cs`
- [x] 2.6 Configure JWT bearer authentication (issuer/audience/signing key from configuration, no cookie auth)
- [x] 2.7 Implement login endpoint: validate credentials, issue JWT access token + refresh token
- [x] 2.8 Implement refresh-token endpoint: exchange a valid refresh token for a new access token
- [x] 2.9 Add a refresh token entity/table modeled on InventoryMSNV's pattern
- [x] 2.10 Add a simple `[Authorize]`-protected "who am I" endpoint for verification

## 3. Backend cross-cutting setup

- [x] 3.1 Add `/health` endpoint (checks app + DB connectivity)
- [x] 3.2 Add Swagger/OpenAPI with JWT bearer auth support in the UI
- [x] 3.3 Configure CORS policy allowing the admin dashboard's dev origin (and a placeholder for prod)
- [x] 3.4 Verify `dotnet build` succeeds across all 4 projects with zero errors

## 4. Backend verification

- [x] 4.1 Start the API; confirm `/health` returns 200
- [x] 4.2 Confirm Swagger UI loads and lists the login/refresh/who-am-i endpoints
- [x] 4.3 Register a test user (or seed one) and confirm login returns a valid JWT
- [x] 4.4 Confirm calling the "who am I" endpoint without a token returns 401, and with a valid token returns the user's identity

## 5. Frontend: TypeScript retrofit + strip WowDash

- [x] 5.1 Add `typescript` package and `tsconfig.json` to `Web/admin-dashboard`; verify `npm run dev` and `npm run build` still succeed with mixed `.jsx`/`.tsx`
- [x] 5.2 Identify and delete unrelated bundled demo pages/components (AI, Crypto, NFT/Gaming, Education LMS, Medical, Chat, Kanban, Gallery, Blog, Marketplace, Email, Calendar, and 19 of 20 `HomePageX` variants), and their now-unused routes in `App.jsx`
- [x] 5.3 Keep/clean up: `MasterLayout`, sidebar/top bar, `SignInPage`, one dashboard home page, reusable Bootstrap table/form primitives
- [x] 5.4 Add placeholder nav entries + routes for Inventory, Sales, Purchasing, Finance, Identity/Admin
- [x] 5.5 Install and configure TanStack Query; create a single API client wrapper (axios or fetch-based) reading the API base URL from environment config
- [x] 5.6 Remove any leftover unused dependencies from `package.json` after stripping (e.g. libraries only used by deleted demo pages)

## 6. Frontend ↔ backend auth integration

- [x] 6.1 Rewrite `SignInPage` to call the real login endpoint via the shared API client
- [x] 6.2 Store the JWT (and refresh token) on successful login; attach the bearer token to subsequent API client requests
- [x] 6.3 Add route protection: redirect unauthenticated users to `SignInPage` for any other route
- [x] 6.4 Handle login failure (invalid credentials) with a visible error message, no navigation

## 7. End-to-end verification

- [x] 7.1 Log in through the real WowDash UI with a real Identity user/password — verified via headless-browser Playwright session (chromium-cli unavailable in this environment, used Playwright directly per the run skill's fallback)
- [x] 7.2 Confirm landing on the dashboard shell, authenticated — confirmed, URL lands on `/`, dashboard widgets render
- [x] 7.3 Confirm a subsequent API call succeeds with the stored token attached — confirmed both via direct curl against `/api/auth/me` (Task 4.4) and via browser: tokens correctly persisted to localStorage and the axios interceptor attaches them
- [x] 7.4 Confirm navigating directly to a protected route while logged out redirects back to `SignInPage` — confirmed, `/` redirected to `/sign-in` when logged out
- [x] 7.5 Confirm all placeholder module nav entries (Inventory, Sales, Purchasing, Finance, Identity/Admin) are visible and navigable — confirmed, all 5 plus Table/Form reference pages present in sidebar and navigate correctly when authenticated

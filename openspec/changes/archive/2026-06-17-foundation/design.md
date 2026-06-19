## Context

LitXusCount targets .NET 10 (LTS) and Clean Architecture layering. The only prior work is research/evaluation, captured in two places: this proposal, and an external planning document the user reviewed and approved before implementation started. The reference project, InventoryMSNV, is a working ASP.NET Core 8→10 MVC app (Razor views, EF Core 8, ASP.NET Core Identity, JWT, SignalR) for the same general business domain (inventory/sales/accounting) — it is used only as a source of proven business-logic patterns (validation rules, Identity configuration shape, audit-field conventions), never as code to copy or as a structural template, since LitXusCount's presentation layer (Web API + separate SPA) and module boundaries differ deliberately from it.

The frontend starting point is WowDash, a commercial admin dashboard template (React 18 + Vite + Bootstrap 5, JavaScript) licensed via the user's Envato Elements subscription. Direct inspection of the downloaded package revealed it is plain JavaScript despite the listing's TypeScript claim, bundles jQuery alongside React, and its "POS & Inventory"/"Finance & Banking" demo labels are just dashboard-widget themes, not real CRUD modules.

## Goals / Non-Goals

**Goals:**
- A buildable, runnable 4-project Clean Architecture backend with strict dependency direction enforced by project references alone.
- Real ASP.NET Core Identity + JWT auth, end-to-end: a real user can log in through the real frontend and call a protected API endpoint.
- A WowDash-derived frontend shell, with TypeScript tooling in place, stripped of irrelevant bundled demos, ready to receive the first real business module.

**Non-Goals:**
- No business module logic yet (Inventory, Sales, etc. — these are separate, later OpenSpec changes).
- No multi-database support (MySQL/PostgreSQL) — SQL Server only, unlike InventoryMSNV, since there is no concrete near-term need and it previously caused a real EF-Core-version mismatch risk during InventoryMSNV's own upgrade.
- No full TypeScript conversion of WowDash's ~150 demo files in this change — only the files that remain after stripping get converted, and only as they're touched.
- No production deployment/CI concerns.

## Decisions

1. **4 separate class library/Web projects, not a single project with folders.** Domain/Application/Infrastructure/API as physically separate `.csproj`s, referencing each other one-directionally. Alternative considered: a single project with namespace-only separation (faster to scaffold, but doesn't *enforce* the dependency rule — a stray `using` could silently violate layering with no build error). Physical project separation makes violations a compile error.

2. **Per-aggregate repositories returning materialized DTOs, never generic `IRepository<T>` or leaked `IQueryable`.** This is carried over directly from the InventoryMSNV review, where `SalesService.GetPaymentGridData()` returned `IQueryable` consumed by 5+ controllers, each chaining further `.Where()`/`.OrderBy()` calls — making the query's real behavior impossible to reason about from any single call site. Alternative considered: generic repository pattern — rejected because it tends to grow a `Query()`/`GetQueryable()` escape hatch that reproduces the same leakage one layer down.

3. **Plain interface + class Application services, no MediatR/CQRS.** Matches the team's familiar "one interface, many methods" shape (as seen in InventoryMSNV's `ICommon`/`ISalesService`), avoiding ceremony (request/response wrapper types, pipeline behaviors) disproportionate to a small/mid-size app, especially while the team is also absorbing a new layering pattern.

4. **TypeScript retrofit on WowDash rather than re-shopping templates again.** Two separate Envato listings claimed TypeScript support that turned out false on inspection (marketing text vs. actual `package.json`/file extensions). Rather than spend more unverifiable search cycles, add `typescript` + `tsconfig.json` to the existing, already-downloaded WowDash codebase and convert files incrementally as they're touched — mechanical, low-risk, doesn't depend on trusting any more listings.

5. **Identity/JWT only, no cookie auth, no Razor login page.** The frontend is a separate SPA, not server-rendered — so the API only needs to issue/validate JWTs (plus a refresh-token flow, modeled on InventoryMSNV's `RefreshToken` table pattern). ASP.NET Core Identity still owns the user/role/password-policy model; only the auth *transport* differs from InventoryMSNV.

6. **SQL Server only for now.** InventoryMSNV supports SQL Server + MySQL (Pomelo) + PostgreSQL (Npgsql) simultaneously, which directly caused a dependency-version mismatch (`Pomelo.EntityFrameworkCore.MySql` 9.0.0 vs `Microsoft.EntityFrameworkCore` 10.0.9, an `NU1608` warning) during InventoryMSNV's own net8→net10 upgrade. LitXusCount has no concrete requirement for multiple providers yet, so this complexity is deferred rather than paid for upfront.

## Risks / Trade-offs

- **[Risk] WowDash's bundled jQuery-dependent libraries (`datatables.net`, `isotope-layout`, `jsvectormap`, `slick-carousel`, `lightgallery.js`) conflict with React's virtual DOM if used carelessly.** → Mitigation: do not introduce new jQuery-dependent code; isolate or replace these specific libraries with React-native equivalents as each affected screen is actually touched, starting with whatever the login/dashboard shell needs (likely none, since those are mostly used by demo pages being deleted).
- **[Risk] Incremental `.jsx`→`.tsx` conversion means the codebase is mixed-language for a while.** → Mitigation: acceptable — Vite supports both extensions simultaneously with no build conflict; the alternative (big-bang rename of ~150 files, most of which are about to be deleted) wastes effort on code that won't survive the strip-down.
- **[Risk] No automated tests are part of this change.** → Mitigation: the Verification section of the proposal/tasks relies on manual end-to-end checks (build succeeds, login works, `/health` responds) — acceptable for a foundation-only change with no business logic yet; revisit test strategy once the first business module (Inventory) is built.
- **[Trade-off] Choosing Bootstrap 5 (WowDash's actual UI library) over Ant Design** (the original preference) because no Envato Elements-available template combined Ant Design with a healthy maintenance/rating signal. Accepted — UI library choice doesn't block the architecture goals of this change.

## Migration Plan

N/A — greenfield project, no existing data or running system to migrate from. EF Core migrations will be created fresh against an empty SQL Server database as part of the `identity-jwt-auth` and `clean-architecture-backend` capability implementation.

## Open Questions

- Solution file format: `.slnx` (newer XML format, net10 SDK supports it) vs classic `.sln` — no functional difference, decide during scaffolding.

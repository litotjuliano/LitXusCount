## Why

The `/admin` (Identity/Admin) nav entry has been a placeholder since the foundation change, and the backend has had no way to manage users or roles since launch — the only users in the system are two hardcoded dev-seeded accounts. Roles already exist as ASP.NET Core Identity roles and are already embedded in the JWT, but nothing checks them: there is no `[Authorize(Roles=...)]`, no `[Authorize(Policy=...)]`, and no permission concept at all. Every later module (Inventory, Sales, Purchasing, Finance) will need real authorization once it has real data worth protecting, so this needs to exist before those modules, not bolted on after.

## What Changes

- Add real CRUD for users: list, create, edit (name/email/roles), deactivate (Identity lockout, not hard delete), admin-triggered password reset. Replaces the two hardcoded dev-seeded accounts as the only way into the system.
- Add real CRUD for roles: list, create, edit (name + assigned permissions), delete — built on existing `IdentityRole`, not a new role table.
- Add a compile-time **permission catalog**, one View/Create/Edit/Delete set per resource (e.g. `Users.View`, `Users.Create`, `Users.Edit`, `Users.Delete`, `Roles.*`, and one set per System Settings entity: `Settings.Currency.*`, `Settings.Category.*`, etc. — `Settings.CompanyInfo` is View/Edit only, since it has no create/delete), assigned to roles as ASP.NET Core Identity role claims (`AspNetRoleClaims`, already part of the Identity schema — no new table). A role's edit screen shows a permission checkbox matrix grouped by resource.
- Add a dynamic permission-based authorization policy provider so API endpoints can declare `[Authorize(Policy = "Permission.Users.Edit")]` without pre-registering every policy by hand.
- **`SuperAdmin` is a protected, built-in role**: cannot be deleted, and implicitly has every permission regardless of which role claims exist for it (so an admin can never accidentally lock everyone out by misconfiguring its claims).
- Replace the `/admin` placeholder with real "Users" and "Roles" pages under the existing Identity/Admin nav entry.
- **Retrofit the same per-entity permissions onto the 9 already-built System Settings endpoints and pages** (Company Info, Currency, VAT Percentage, Email Config, Payment Type, Payment Status, Customer Type, Category, Unit of Measure) — they move off bare `[Authorize]` onto `[Authorize(Policy = "Permission.Settings.{Entity}.{Action}")]`, and the frontend hides Add/Edit/Delete controls (and sidebar entries) a user lacks permission for, rather than just disabling them. This establishes the pattern every future module (Inventory, Sales, Purchasing, Finance) is expected to follow from day one.

## Capabilities

### New Capabilities
- `user-management`: User CRUD (list/create/edit/deactivate), admin-triggered password reset, and per-user role assignment.
- `role-permission-management`: Role CRUD, the permission catalog (including the System Settings entities), role-claim-based permission assignment, the dynamic permission policy provider, and the `SuperAdmin` protection rule.

### Modified Capabilities
- `identity-jwt-auth`: the JWT issued at login SHALL also carry the user's effective permission claims (aggregated from all roles the user belongs to), not just role names.
- `admin-dashboard-shell`: the Identity/Admin nav entry gains real "Users" and "Roles" pages, replacing the placeholder; the System Settings nav entries and page controls become permission-aware (hidden, not just disabled, when the user lacks the relevant permission).
- `system-lookup-management`: the 8 lookup entities' endpoints require per-entity, per-action permissions instead of bare `[Authorize]`.
- `company-info-settings`: the Company Info endpoint requires `Settings.CompanyInfo.View`/`Edit` instead of bare `[Authorize]`.

## Impact

- Backend: no new entities/tables for permissions (role claims reuse the existing `AspNetRoleClaims` Identity table); new Application/Infrastructure services for user and role management; new API controllers; a new dynamic authorization policy provider wired into `Program.cs`; the 9 System Settings controllers gain per-action policy attributes (4 of them via a new imperative-authorization check in `LookupControllerBase<TService>`, since that shared base can't use a single static policy across 5 differently-permissioned subclasses).
- Frontend: new "Users" and "Roles" pages under Identity/Admin, reusing the `PaginatedTable`/`usePaginatedQuery` pattern established in `list-pagination-and-filtering`; a new `usePermissions` hook (manual JWT-payload decode, no new dependency) used to gate System Settings page controls and sidebar entries, and to gate the new Users/Roles pages themselves.

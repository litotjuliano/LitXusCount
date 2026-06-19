## 1. Permission catalog and policy infrastructure

- [x] 1.1 Add `Permissions` static class to `LitXusCount.Application` with nested `Users`/`Roles` classes (`View`/`Create`/`Edit`/`Delete` constants), and a nested `Settings` class containing one nested class per System Settings entity (`CompanyInfo` with `View`/`Edit` only; `Currency`, `VatPercentage`, `EmailConfig`, `PaymentType`, `PaymentStatus`, `CustomerType`, `Category`, `UnitOfMeasure` each with `View`/`Create`/`Edit`/`Delete`) — e.g. `Permissions.Settings.Currency.Delete = "Settings.Currency.Delete"`
- [x] 1.2 Add a helper to enumerate all catalog permission strings via **recursive** reflection (walks nested classes at any depth — needed for `Settings.{Entity}.{Action}`, not just the 1-level `Users.{Action}` shape) for the role-edit checkbox matrix and for seeding
- [x] 1.3 Add `PermissionRequirement` + `PermissionAuthorizationHandler` (succeeds if the user has a `"permission"` claim matching the required value, OR has the `SuperAdmin` role claim)
- [x] 1.4 Add `PermissionPolicyProvider : IAuthorizationPolicyProvider` that creates a policy on-demand for any policy name of the form `Permission.{Name}` (works unchanged for deeper names like `Permission.Settings.Currency.Delete` — it just strips the `Permission.` prefix and checks the remainder as a claim value)
- [x] 1.5 Wire `PermissionPolicyProvider` and `PermissionAuthorizationHandler` into DI in `Program.cs`/`DependencyInjection.cs`

## 2. JWT permission claims

- [x] 2.1 Update `AuthService.IssueTokensAsync` to call `RoleManager.GetClaimsAsync(role)` for each of the user's roles, collect all `"permission"`-typed claims, de-duplicate, and add them to the token's claims
- [x] 2.2 Confirm `AuthResult` (or a new field on it) is unaffected/extended as needed — permissions live in the JWT itself, not necessarily duplicated in the login response body unless useful for the frontend to avoid decoding the JWT

## 3. Role management (backend)

- [x] 3.1 Add `RoleDto`/`RoleUpsertDto`/`RoleDetailDto` (with assigned permission list) in `LitXusCount.Application`
- [x] 3.2 Add `IRoleService`/`RoleService`: `ListAsync(PagedQuery)`, `GetAsync(id)` (including its permissions), `CreateAsync`, `EditAsync` (name + permission set), `DeleteAsync` — `DeleteAsync` and permission-removal on `EditAsync` SHALL reject `SuperAdmin`
- [x] 3.3 Add `RolesController` with `[Authorize(Policy = "Permission.Roles.View")]` on reads and `Permission.Roles.Create`/`Edit`/`Delete` on writes
- [x] 3.4 Seed `SuperAdmin` with every catalog permission and `Admin` with a starter subset (`Users.View`, `Roles.View`, plus full View/Create/Edit/Delete on every System Settings entity) in `Program.cs`'s existing dev-seed block

## 4. User management (backend)

- [x] 4.1 Add `UserDto`/`UserUpsertDto` (display name, email, role names, active/locked status) in `LitXusCount.Application`
- [x] 4.2 Add `IUserManagementService`/`UserManagementService`: `ListAsync(PagedQuery)` (active by default, with an option to include deactivated), `GetAsync(id)`, `CreateAsync` (creates `ApplicationUser` + assigns roles), `EditAsync` (display name/email/roles), `DeactivateAsync` (sets `LockoutEnd = DateTimeOffset.MaxValue`, `LockoutEnabled = true`), `ReactivateAsync` (clears `LockoutEnd`), `AdminResetPasswordAsync` (generate reset token internally and immediately consume it, or use `UserManager.RemovePasswordAsync`+`AddPasswordAsync`)
- [x] 4.3 Add `UsersController` with `[Authorize(Policy = "Permission.Users.View")]` on reads and `Permission.Users.Create`/`Edit`/`Delete` on writes/deactivate/reset-password
- [x] 4.4 Confirm deactivated users are correctly rejected at login (`AuthService.LoginAsync` already calls `IsLockedOutAsync` — verify, don't reimplement)

## 5. Retrofit permissions onto System Settings (backend)

- [x] 5.1 Add `[Authorize(Policy = "Permission.Settings.CompanyInfo.View")]`/`...Edit` to `CompanyInfoController`'s `Get`/`Edit` actions, replacing bare `[Authorize]`
- [x] 5.2 Add per-action `[Authorize(Policy = "Permission.Settings.Currency.{Action}")]` to `CurrenciesController`, replacing bare `[Authorize]`
- [x] 5.3 Same for `VatPercentagesController` (`Settings.VatPercentage.*`) and `EmailConfigsController` (`Settings.EmailConfig.*`)
- [x] 5.4 Add `protected abstract string ResourceName { get; }` to `LookupControllerBase<TService>`, inject `IAuthorizationService`, and replace its `[Authorize]` with imperative `await authorizationService.AuthorizeAsync(User, $"Permission.Settings.{ResourceName}.{Action}")` (→ `Forbid()` on failure) in each of List/Get/Create/Edit/Delete — a static attribute can't vary per subclass, which is why this one needs the imperative check instead of the other 4 controllers' declarative attributes
- [x] 5.5 Override `ResourceName` in each of the 5 `LookupControllerBase` subclasses (`"PaymentType"`, `"PaymentStatus"`, `"CustomerType"`, `"Category"`, `"UnitOfMeasure"`) in `SimpleLookupControllers.cs`

## 6. Backend verification

- [x] 6.1 `dotnet build` succeeds with zero errors across all 4 projects
- [x] 6.2 Create a role with a subset of permissions, create a user assigned to it, log in as that user, and confirm the JWT (decode it) contains exactly that role's permission claims
- [x] 6.3 Confirm a user lacking `Users.Delete` gets 403 from a delete-user request, and a user holding it succeeds
- [x] 6.4 Confirm `SuperAdmin` cannot be deleted, and a `SuperAdmin` user succeeds against an endpoint requiring a permission `SuperAdmin`'s claims don't actually list
- [x] 6.5 Deactivate a user via the API, confirm their subsequent login attempt is rejected, then reactivate and confirm login succeeds again
- [x] 6.6 Confirm the user list excludes deactivated users by default and includes them when explicitly requested
- [x] 6.7 Confirm a user with only `Settings.Currency.View` gets 200 on `GET currencies` but 403 on POST/PUT/DELETE
- [x] 6.8 Confirm a user with `Settings.Currency.Delete` but not `Settings.Category.Delete` gets 403 deleting a category — proves the imperative `ResourceName` check actually differentiates between the 5 shared-base entities, not just uniformly pass/fail

## 7. Frontend: Identity/Admin pages

- [x] 7.1 Replace the `/admin` placeholder route with `/admin/users` and `/admin/roles`, update the sidebar to a real "Identity / Admin" group with "Users" and "Roles" entries (mirroring the System Settings dropdown pattern)
- [x] 7.2 Add API modules (`src/api/admin/users.ts`, `src/api/admin/roles.ts`) and hooks (`useUserManagement`, `useRoleManagement`) following the `usePaginatedQuery`/mutation pattern from System Settings
- [x] 7.3 Build the Users page: `PaginatedTable` list, create/edit form (display name, email, password on create, multi-select roles), deactivate/reactivate action, admin password-reset action
- [x] 7.4 Build the Roles page: `PaginatedTable` list, create/edit form (name + a permission checkbox matrix grouped by resource: Users, Roles, then each System Settings entity), delete action (hidden for `SuperAdmin`)
- [x] 7.5 Surface API validation/authorization errors (e.g. attempting to delete `SuperAdmin`, duplicate role name) as inline form errors, consistent with the System Settings pages

## 8. Frontend: permission-aware UI

- [x] 8.1 Add `api/jwt.ts`: decode a JWT's payload segment (split on `.`, base64url-decode, `JSON.parse`) — no new npm dependency
- [x] 8.2 Add `hook/usePermissions.ts`: reads the access token via `authStorage.getAccessToken()`, decodes it, normalizes the `"permission"` claim (string or array) into a `Set<string>`, and exposes `hasPermission(name: string): boolean`
- [x] 8.3 In `MasterLayout.tsx`, hide each System Settings sub-nav entry (and the Users/Roles entries) when the user lacks that entity's `View` permission
- [x] 8.4 In each of the 9 System Settings layer components (`LookupSettingsLayer`, `CurrencySettingsLayer`, `VatPercentageSettingsLayer`, `EmailConfigSettingsLayer`, `CompanyInfoSettingsLayer`), gate the Add/Create form on `Create`, the per-row Edit button on `Edit`, and the per-row Delete button on `Delete` (`CompanyInfoSettingsLayer` only has `View`/`Edit` to check)
- [x] 8.5 Apply the same gating to the new Users and Roles pages' Add/Edit/Delete controls

## 9. End-to-end verification

- [x] 9.1 Log in as the seeded `SuperAdmin` user, navigate to Roles, create a new role with a couple of permissions checked, and confirm it's saved
- [x] 9.2 Create a new user, assign the new role, and confirm the user appears in the Users list
- [x] 9.3 Log in as the new user and confirm they can/cannot access the areas implied by their assigned permissions
- [x] 9.4 Deactivate the new user as `SuperAdmin`, confirm that user can no longer log in, then reactivate and confirm they can again
- [x] 9.5 Confirm the Roles page does not allow deleting `SuperAdmin` (action hidden or rejected with a visible message)
- [x] 9.6 Log in as a user whose role has only `Settings.Currency.View`, confirm the Manage Currency page shows the list with no Add form and no Edit/Delete buttons, and confirm a sidebar entry for an entity they have zero permissions on doesn't render at all

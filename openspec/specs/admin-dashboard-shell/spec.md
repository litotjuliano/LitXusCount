## Purpose

Defines the stripped-down, TypeScript-enabled WowDash admin dashboard shell: layout, routing, navigation placeholders for planned business modules, and the centralized API client/data-fetching pattern, with no business data yet.

## Requirements

### Requirement: TypeScript-enabled build
The admin dashboard project SHALL compile with TypeScript tooling (a `typescript` dependency and `tsconfig.json` present and active in the Vite build), even though the codebase remains a mix of `.jsx`/`.tsx` files during incremental conversion.

#### Scenario: TypeScript compiles alongside existing JavaScript
- **WHEN** `npm run build` (or `npm run dev`) is executed
- **THEN** the build succeeds with TypeScript checking active, and existing `.jsx` files continue to work unmodified

### Requirement: Stripped navigation shell
The admin dashboard SHALL retain only the master layout, sidebar/top bar, login screen, routing setup, and reusable Bootstrap table/form primitives from the original WowDash template. All unrelated bundled demo pages (AI, Crypto, NFT/Gaming, Education LMS, Medical, Chat, Kanban, Gallery, Blog, Marketplace, and 19 of the 20 generic `HomePageX` dashboard variants) SHALL be removed.

#### Scenario: Unrelated demo routes are gone
- **WHEN** the application's route table is inspected after stripping
- **THEN** no routes exist for the removed demo pages (e.g. `/kanban`, `/gallery`, `/marketplace`, `/blog`)

#### Scenario: Core shell still renders
- **WHEN** the application is loaded in a browser
- **THEN** the sidebar, top bar, and a dashboard home page render without errors

### Requirement: Placeholder navigation for planned business modules
The sidebar navigation SHALL include placeholder entries for each planned business module: Inventory, Sales, Purchasing, and Finance — even though their pages contain no real data yet. Identity/Admin is no longer a placeholder (see "Identity/Admin navigation group").

#### Scenario: All planned modules appear in navigation
- **WHEN** the sidebar is rendered
- **THEN** entries for Inventory, Sales, Purchasing, and Finance are all present and clickable, each navigating to a placeholder page

### Requirement: Identity/Admin navigation group
The sidebar navigation SHALL include a real (non-placeholder) "Identity / Admin" entry containing "Users" and "Roles" pages, each navigating to a working list/edit page wired to the corresponding API.

#### Scenario: Identity/Admin group renders with Users and Roles
- **WHEN** the sidebar is rendered for an authenticated user
- **THEN** an "Identity / Admin" entry is present with "Users" and "Roles" sub-entries, each clickable and navigating to a real page

#### Scenario: Users page lists, creates, edits, and deactivates
- **WHEN** a user with sufficient permissions navigates to the Users page
- **THEN** the page lists existing users fetched from the API, and supports creating, editing, and deactivating/reactivating users through that same API

#### Scenario: Roles page lists, creates, edits permissions, and deletes
- **WHEN** a user with sufficient permissions navigates to the Roles page
- **THEN** the page lists existing roles, and supports creating a role, editing its assigned permissions via a checkbox matrix, and deleting it (except `SuperAdmin`)

### Requirement: Permission-aware UI hides, not disables, unavailable controls
For every System Settings entity and the new Users/Roles pages, the frontend SHALL hide (not show-but-disable) any sidebar entry, Add/Create control, Edit control, or Delete control that the logged-in user's permissions do not allow, based on the `"permission"` claims embedded in their JWT.

#### Scenario: Sidebar entry is hidden without view permission
- **WHEN** a user lacks `Settings.Currency.View`
- **THEN** the "Manage Currency" sidebar entry does not render

#### Scenario: Add control is hidden without create permission
- **WHEN** a user has `Settings.Currency.View` but not `Settings.Currency.Create`
- **THEN** the Manage Currency page shows the existing list but no Add/Create form

#### Scenario: Edit and Delete controls are hidden per-row without the corresponding permission
- **WHEN** a user has `Settings.Currency.View` but neither `Settings.Currency.Edit` nor `Settings.Currency.Delete`
- **THEN** the Manage Currency page's rows show no Edit or Delete buttons

### Requirement: System Settings navigation group
The sidebar navigation SHALL include a "System Settings" group containing real (non-placeholder) entries: Company Info, Email Config, Manage Currency, Payment Type, Payment Status, Customer Type, VAT Percentage, Categories, and Units of Measure — each navigating to a working list/edit page wired to the corresponding API.

#### Scenario: System Settings group renders with all entries
- **WHEN** the sidebar is rendered for an authenticated user
- **THEN** a "System Settings" group is present with entries for Company Info, Email Config, Manage Currency, Payment Type, Payment Status, Customer Type, VAT Percentage, Categories, and Units of Measure, each clickable and navigating to a real page

#### Scenario: Lookup pages list, create, edit, and delete
- **WHEN** a user navigates to one of the eight CRUD lookup pages (e.g. Manage Currency)
- **THEN** the page lists existing active rows fetched from the API, and supports creating, editing, and deleting rows through that same API

#### Scenario: Company Info page is edit-only
- **WHEN** a user navigates to the Company Info page
- **THEN** the page shows the single existing `CompanyInfo` record and allows editing it, with no create or delete action available

### Requirement: Reusable paginated/sortable/filterable list pattern
The admin dashboard SHALL use one reusable `PaginatedTable` component and one reusable `usePaginatedQuery` hook for every list page, rather than each page implementing its own fetch/search/sort/page state. The hook SHALL own `page`, `pageSize`, `search`, `sortBy`, and `sortDescending` state, debounce `search` input, and include all five as part of the TanStack Query cache key so each combination is cached independently.

#### Scenario: List page renders search, sortable columns, and page controls
- **WHEN** a list page built on `PaginatedTable`/`usePaginatedQuery` is rendered
- **THEN** it shows a search input, clickable/sortable column headers, and page navigation controls (current page, total pages, page-size selector)

#### Scenario: Search input is debounced
- **WHEN** a user types into the search input
- **THEN** the underlying API request is not sent on every keystroke, but after a short pause in typing (debounced)

#### Scenario: Changing page, sort, or search refetches the list
- **WHEN** a user changes the page number, clicks a sortable column header, or changes the search term
- **THEN** the list refetches from the API with the updated `page`/`sortBy`/`sortDescending`/`search` parameters and re-renders the new result set

### Requirement: Centralized API client with TanStack Query
The admin dashboard SHALL use a single API client wrapper (axios or fetch-based) combined with TanStack Query for server-state caching and mutations. No other competing data-fetching pattern SHALL be introduced.

#### Scenario: API calls go through the shared client
- **WHEN** any component needs to call the backend API
- **THEN** it does so through the shared API client wrapper and TanStack Query hooks, not ad hoc `fetch`/`axios` calls scattered in component code

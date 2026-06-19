## ADDED Requirements

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
The sidebar navigation SHALL include placeholder entries for each planned business module: Inventory, Sales, Purchasing, Finance, and Identity/Admin — even though their pages contain no real data yet.

#### Scenario: All planned modules appear in navigation
- **WHEN** the sidebar is rendered
- **THEN** entries for Inventory, Sales, Purchasing, Finance, and Identity/Admin are all present and clickable, each navigating to a placeholder page

### Requirement: Centralized API client with TanStack Query
The admin dashboard SHALL use a single API client wrapper (axios or fetch-based) combined with TanStack Query for server-state caching and mutations. No other competing data-fetching pattern SHALL be introduced.

#### Scenario: API calls go through the shared client
- **WHEN** any component needs to call the backend API
- **THEN** it does so through the shared API client wrapper and TanStack Query hooks, not ad hoc `fetch`/`axios` calls scattered in component code

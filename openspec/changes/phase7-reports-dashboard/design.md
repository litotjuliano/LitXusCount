## Context

All transactional data exists by Phase 7. Reports are read-only query projections over that data — no new entities needed. The live dashboard replaces the current static placeholder. InventoryMSNV delivers reports as server-rendered Razor views with DataTables; LitXusCount delivers them as JSON API endpoints consumed by React pages.

## Goals / Non-Goals

**Goals:**
- Six report endpoints: Sales, Customer Sales, Purchases, Stock, Expense Summary, Income Summary
- Live dashboard summary endpoint (KPI cards + trend data)
- Date range, entity (customer/supplier), and status filters on relevant reports
- Frontend report pages with filter UI and sortable tables
- Dashboard home page with summary cards and at least one trend chart

**Non-Goals:**
- PDF/Excel export (can be added as an enhancement — browser print-to-PDF covers the immediate need)
- Scheduled/emailed reports — future enhancement
- Custom report builder — future enhancement
- Report caching/materialized views — future enhancement (add if query performance requires it)

## Decisions

### 1. Reports are dedicated read-only query endpoints, not reused list endpoints
**Decision:** Each report gets its own controller action with its own filter DTO and response shape, separate from the CRUD list endpoints used by settings pages.

**Rationale:** Report projections join multiple tables, compute aggregates, and shape data differently from CRUD responses. Forcing them through `PagedResult<T>` would be awkward. Report endpoints return the full filtered result set (not paged) since reports are typically printed/exported.

**Alternative considered:** Reuse CRUD list endpoints with extra filter params — rejected because the response shape is fundamentally different (aggregated totals, joined fields).

### 2. Dashboard aggregates computed server-side in a single endpoint
**Decision:** `GET /api/dashboard/summary` returns all KPI values and trend arrays in one response to minimize round trips on page load.

**Rationale:** Dashboard loads on every login. One call is faster and simpler than 5–6 separate calls. Data freshness of ~1 minute is acceptable — no real-time requirement.

### 3. Chart library: Recharts
**Decision:** Use Recharts for frontend charts (already a common React chart library, lightweight, works with TanStack Query data).

**Rationale:** No chart library is currently in the project. Recharts is the most widely used React-native chart library, has TypeScript support, and requires no additional build config.

**Alternative considered:** Chart.js (via react-chartjs-2) — heavier bundle, more complex API for the charts needed.

### 4. Report filters are query parameters, not request body
**Decision:** All report endpoints use GET with query string filters (`?from=2026-01-01&to=2026-06-30&customerId=5`).

**Rationale:** Reports are read-only, bookmarkable, and cache-friendly. GET with query params is the correct HTTP semantic.

## Risks / Trade-offs

- **[Risk] Report queries slow on large datasets** → Mitigation: Ensure FK columns used in WHERE/JOIN clauses have indexes (added in this migration); add date-range indexes on invoice/transaction date columns.
- **[Risk] Dashboard summary becomes slow as data grows** → Mitigation: Start with direct queries; move to a materialized/cached summary if needed in a future performance pass.
- **[Risk] Recharts bundle size** → Mitigation: Use tree-shaking (only import the chart types used); Recharts is ~180KB gzipped which is acceptable.

## Migration Plan

1. No new entity tables
2. Add performance indexes on date columns (SalesInvoice.CreatedAt, PurchaseInvoice.CreatedAt, AccTransaction.CreatedDate) and FK columns used in report JOINs
3. Install Recharts: `npm install recharts` in `Web/admin-dashboard`

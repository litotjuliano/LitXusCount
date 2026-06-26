## Context

InventoryMSNV uses `AccAccount` as a cash/bank ledger — not a full double-entry GL (that's `GlAccount`). Each account has a running Credit, Debit, and Balance. Operations (Deposit, Expense, Transfer) update these totals atomically and append an `AccTransaction` audit row. Sales and Purchasing payment recording (Phase 4/5) also updates these balances when a payment mode is saved or deleted.

## Goals / Non-Goals

**Goals:**
- CRUD for AccAccount (cash/bank/credit accounts)
- Deposit, Expense, Transfer operations with atomic balance update
- AccTransaction audit log — one row per balance-changing event
- All-active AccAccount lookup endpoint for use in Phase 4/5 payment recording

**Non-Goals:**
- Full double-entry GL journal posting (that is GlAccount's domain, future Accounting module)
- Bank reconciliation
- Multi-currency per account (accounts hold a single balance; currency handled at invoice level)

## Decisions

### 1. Balance maintained as denormalized columns on AccAccount
**Decision:** `AccAccount.Credit`, `AccAccount.Debit`, and `AccAccount.Balance` are updated in-place on every operation rather than computed from the AccTransaction log.

**Rationale:** Mirrors InventoryMSNV exactly. Avoids expensive aggregate queries on the transaction log for every balance display. The log remains the audit trail; the columns are the fast-read cache.

**Alternative considered:** Compute balance from transaction log sum — rejected for performance and complexity at report time.

### 2. AccTransaction is append-only (no edit/delete)
**Decision:** `AccTransaction` rows are never edited or deleted. Reversals are new rows (e.g. a deleted Deposit creates a reversal AccTransaction row).

**Rationale:** Audit integrity — every balance change must be traceable. Mirrors InventoryMSNV's `UpdateAccAccountInDeletePaymentModeHistory` pattern which creates a reversal rather than removing the original.

### 3. Transfer uses sender/receiver FK pair on a single row
**Decision:** `AccTransfer` holds both `SenderId` and `ReceiverId` FKs in one row, plus amount and date. It triggers two AccTransaction rows (debit sender, credit receiver) and updates both AccAccount balances.

**Rationale:** Atomicity — both sides of the transfer are committed together in one `SaveChangesAsync`. Simpler to display transfer history than joining two separate rows.

## Risks / Trade-offs

- **[Risk] Balance desync if transactions are deleted directly in DB** → Mitigation: Balance columns are only ever updated through service methods, never raw SQL outside migrations.
- **[Risk] Concurrent balance updates** → Mitigation: Use EF Core optimistic concurrency (row version or timestamp) on `AccAccount` balance columns in a future hardening pass; acceptable at current single-user/low-concurrency scale.

## Migration Plan

1. Add EF Core migration for 5 new tables
2. Migration auto-runs on startup — no manual step
3. Seed a default "Cash" account in `SystemSettingsSeeder` so Phase 4/5 have a target account available immediately

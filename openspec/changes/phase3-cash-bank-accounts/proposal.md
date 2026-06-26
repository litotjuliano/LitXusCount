## Why

Sales and Purchasing invoice payments must be recorded against a cash, bank, or credit account. Before Phase 4 (Sales) and Phase 5 (Purchasing) can be built, the account ledger system must exist. This phase migrates InventoryMSNV's `AccAccount`, `AccTransaction`, `AccDeposit`, `AccExpense`, and `AccTransfer` entities into LitXusCount's Clean Architecture.

## What Changes

- Add `AccAccount` entity: named cash/bank/credit accounts with running Credit, Debit, and Balance totals
- Add `AccTransaction` entity: immutable audit log of every debit/credit movement against an account
- Add `AccDeposit` entity: manual deposits into an account
- Add `AccExpense` entity: expenses paid out from an account
- Add `AccTransfer` entity: fund transfers between two accounts
- Add balance update logic: Deposit → credits account; Expense → debits account; Transfer → debits sender, credits receiver
- Add API controllers and frontend CRUD pages for all 5 entities
- Add "Accounts" sidebar navigation group

## Capabilities

### New Capabilities

- `acc-account-management`: CRUD for cash/bank/credit accounts with running balance tracking
- `acc-transactions`: Deposit, Expense, and Transfer operations that update account balances atomically; AccTransaction audit log per operation

### Modified Capabilities

- `admin-dashboard-shell`: Add "Accounts" sidebar group with Accounts, Deposits, Expenses, Transfers entries

## Impact

- **Backend:** 5 new domain entities, EF Core config, service interfaces + implementations, DI registration, API controllers
- **Frontend:** API client modules, TanStack Query hooks, layer components, pages, sidebar nav
- **Database:** New migration adding 5 tables
- **Dependency:** Phase 4 (Sales) and Phase 5 (Purchasing) depend on `AccAccount` being available for payment recording

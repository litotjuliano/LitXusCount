# Phase 3: Cash & Bank Accounts — Implementation Tasks

## Section 1: Domain Entities

- [x] 1.1 Create `AccAccount` entity in `src/LitXusCount.Domain/Entities/` with Code, AccountName, AccountNumber, Description, Credit, Debit, Balance, audit fields, IsActive
- [x] 1.2 Create `AccTransaction` entity with AccountId FK, Type (enum: Deposit/Expense/TransferIn/TransferOut/SalesPayment/PurchasePayment/Reversal), Credit, Debit, Amount, Reference, Description, audit fields
- [x] 1.3 Create `AccDeposit` entity with AccountId FK, DepositDate, Amount, Note, IsActive, audit fields
- [x] 1.4 Create `AccExpense` entity with AccountId FK, Name, ExpenseDate, Amount, Note, IsActive, audit fields
- [x] 1.5 Create `AccTransfer` entity with SenderId FK, ReceiverId FK, TransferDate, Amount, Note, IsActive, audit fields

## Section 2: Infrastructure — EF Core Configuration

- [x] 2.1 Add `AccAccountConfiguration` in `src/LitXusCount.Infrastructure/Persistence/Configuration/` — filtered unique index on Code where IsActive=true
- [x] 2.2 Add `AccTransactionConfiguration` — FK to AccAccount, no delete endpoint (append-only)
- [x] 2.3 Add `AccDepositConfiguration`, `AccExpenseConfiguration`, `AccTransferConfiguration`
- [x] 2.4 Register all 5 DbSets in `ApplicationDbContext`
- [x] 2.5 Add and run EF Core migration: `AddCashBankAccountTables`

## Section 3: Application Layer — Service Interfaces & Commands

- [x] 3.1 Create `IAccAccountService` with: `GetPagedAsync`, `GetAllActiveAsync`, `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`
- [x] 3.2 Create `IAccDepositService` with: `GetPagedAsync`, `GetByIdAsync`, `CreateAsync`, `DeleteAsync` (reversal on delete)
- [x] 3.3 Create `IAccExpenseService` with same shape as Deposit
- [x] 3.4 Create `IAccTransferService` with: `GetPagedAsync`, `GetByIdAsync`, `CreateAsync`, `DeleteAsync` (reversal on delete)
- [x] 3.5 Define DTOs: `AccAccountDto`, `AccAccountCreateDto`, `AccAccountUpdateDto`, `AccDepositDto`, `AccDepositCreateDto`, `AccExpenseDto`, `AccExpenseCreateDto`, `AccTransferDto`, `AccTransferCreateDto`, `AccTransactionDto`

## Section 4: Application Layer — Service Implementations

- [x] 4.1 Implement `AccAccountService` — CRUD only, no balance modification via edit endpoint
- [x] 4.2 Implement `AccDepositService.CreateAsync` — saves AccDeposit + updates AccAccount.Credit and Balance + appends AccTransaction (Type=Deposit)
- [x] 4.3 Implement `AccDepositService.DeleteAsync` — soft-deletes AccDeposit + reverses AccAccount.Credit and Balance + appends reversal AccTransaction
- [x] 4.4 Implement `AccExpenseService.CreateAsync` — saves AccExpense + updates AccAccount.Debit + decrements AccAccount.Balance + appends AccTransaction (Type=Expense)
- [x] 4.5 Implement `AccExpenseService.DeleteAsync` — soft-deletes + reverses balance + appends reversal AccTransaction
- [x] 4.6 Implement `AccTransferService.CreateAsync` — validates SenderId ≠ ReceiverId, saves AccTransfer, debits sender, credits receiver, appends two AccTransaction rows — all in one atomic SaveChangesAsync
- [x] 4.7 Implement `AccTransferService.DeleteAsync` — reverses both account balances, appends two reversal AccTransaction rows

## Section 5: Permissions

- [x] 5.1 Add `Accounts` static class to `Permissions.cs` with View, Create, Edit, Delete constants for each sub-module: Account, Deposit, Expense, Transfer
- [x] 5.2 Seed Accounts permissions into the SuperAdmin role in `SystemSettingsSeeder` (or confirm reflection-based `Permissions.All` covers it automatically)

## Section 6: API Controllers

- [x] 6.1 Create `AccountsController` at `src/LitXusCount.API/Controllers/Accounts/AccountsController.cs` — routes: GET (paged), GET `/all-active`, GET `/{id}`, POST, PUT `/{id}`, DELETE `/{id}`; permission attributes on each action
- [x] 6.2 Create `DepositsController` — GET (paged, filtered by accountId), GET `/{id}`, POST, DELETE `/{id}`
- [x] 6.3 Create `ExpensesController` — GET (paged, filtered by accountId), GET `/{id}`, POST, DELETE `/{id}`
- [x] 6.4 Create `TransfersController` — GET (paged), GET `/{id}`, POST, DELETE `/{id}`
- [x] 6.5 Register `IAccAccountService`, `IAccDepositService`, `IAccExpenseService`, `IAccTransferService` in `DependencyInjection.cs`

## Section 7: Frontend — API Client Layer

- [x] 7.1 Create `Web/admin-dashboard/src/api/accounts/accounts.ts` — typed axios calls for all AccountsController endpoints
- [x] 7.2 Create `deposits.ts`, `expenses.ts`, `transfers.ts` in the same folder

## Section 8: Frontend — Hooks

- [x] 8.1 Create `useAccounts.ts` — `useAccountsQuery` (paged), `useAllActiveAccounts`, `useCreateAccount`, `useUpdateAccount`, `useDeleteAccount`
- [x] 8.2 Create `useDeposits.ts`, `useExpenses.ts`, `useTransfers.ts` with analogous hooks

## Section 9: Frontend — Pages & Components

- [x] 9.1 Create `AccountsPage.tsx` — PaginatedTable with Code/Name/Balance columns, Create/Edit modal with Code, AccountName, AccountNumber, Description fields; balance columns read-only
- [x] 9.2 Create `DepositsPage.tsx` — table with Account/Date/Amount/Note columns; create form with AccountId dropdown (from all-active), Date, Amount, Note; delete with reversal confirmation
- [x] 9.3 Create `ExpensesPage.tsx` — same shape as Deposits with Name field added
- [x] 9.4 Create `TransfersPage.tsx` — table with From/To/Date/Amount columns; create form with two AccountId dropdowns, Amount, Date, Note; validation that From ≠ To

## Section 10: Frontend — Sidebar Navigation

- [x] 10.1 Add "Accounts" group to sidebar with entries: Accounts, Deposits, Expenses, Transfers; permission-gated

## Section 11: Seed Data

- [x] 11.1 Add a default "Cash" `AccAccount` row in `SystemSettingsSeeder` so Phase 4/5 payment recording has a target account immediately

## Section 12: Verification

- [x] 12.1 `dotnet build` passes across all 4 projects with no warnings
- [ ] 12.2 Migration runs successfully on startup — 5 new tables created
- [ ] 12.3 Create an AccAccount via Swagger, confirm Credit=Debit=Balance=0
- [ ] 12.4 Record a Deposit — confirm AccAccount.Balance increases; AccTransaction row appended
- [ ] 12.5 Delete the Deposit — confirm Balance restored; reversal AccTransaction row appended
- [ ] 12.6 Record a Transfer — confirm both accounts updated atomically; two AccTransaction rows appended
- [ ] 12.7 Confirm edit endpoint does not change Credit/Debit/Balance
- [ ] 12.8 Frontend pages load, CRUD works, Accounts group visible in sidebar for permitted user

## ADDED Requirements

### Requirement: AccAccount CRUD with running balance
The system SHALL provide CRUD management for `AccAccount` entities. Each account SHALL carry `AccountName`, `AccountNumber`, `Description`, `Credit` (total credited), `Debit` (total debited), and `Balance` (Credit minus Debit), plus standard audit fields. `Credit`, `Debit`, and `Balance` SHALL only be updated through service operations (Deposit, Expense, Transfer, Payment recording) — never directly via the CRUD edit endpoint. Listing SHALL follow the standard `PagedQuery`/`PagedResult<T>` contract.

#### Scenario: AccAccount can be created
- **WHEN** an authorized user creates an account with a name and account number
- **THEN** a new AccAccount row is persisted with Credit=0, Debit=0, Balance=0

#### Scenario: AccAccount balance fields are read-only via edit endpoint
- **WHEN** an authorized user submits an edit for an AccAccount including modified Credit/Debit/Balance values
- **THEN** those fields are ignored and the balance remains unchanged by the edit operation

#### Scenario: All-active endpoint returns full account list
- **WHEN** a GET request is made to `/api/accounts/all-active`
- **THEN** the response is an array of all active AccAccounts for use as dropdown sources in payment recording

### Requirement: Deposit operation credits an account
The system SHALL provide a Deposit operation that records an `AccDeposit` (AccountId, DepositDate, Amount, Note), appends an `AccTransaction` audit row (Type="Deposit", Credit=Amount), and increments `AccAccount.Credit` and `AccAccount.Balance` by the deposit amount — all in a single atomic operation.

#### Scenario: Deposit increases account balance
- **WHEN** an authorized user submits a deposit of 500 to an account with Balance=1000
- **THEN** AccAccount.Balance becomes 1500, AccAccount.Credit increases by 500, and one AccTransaction row is appended

#### Scenario: Deposit can be soft-deleted with reversal
- **WHEN** an authorized user deletes an existing deposit
- **THEN** AccAccount.Credit and Balance are decremented by the deposit amount, and a reversal AccTransaction row is appended

### Requirement: Expense operation debits an account
The system SHALL provide an Expense operation that records an `AccExpense` (AccountId, Name, ExpenseDate, Amount, Note), appends an `AccTransaction` audit row (Type="Expense", Debit=Amount), and increments `AccAccount.Debit` and decrements `AccAccount.Balance` by the expense amount atomically.

#### Scenario: Expense decreases account balance
- **WHEN** an authorized user submits an expense of 200 from an account with Balance=1000
- **THEN** AccAccount.Balance becomes 800, AccAccount.Debit increases by 200, and one AccTransaction row is appended

#### Scenario: Expense can be soft-deleted with reversal
- **WHEN** an authorized user deletes an existing expense
- **THEN** AccAccount.Debit and Balance are restored, and a reversal AccTransaction row is appended

### Requirement: Transfer operation moves funds between accounts
The system SHALL provide a Transfer operation that records an `AccTransfer` (SenderId, ReceiverId, TransferDate, Amount, Note), debits the sender account, credits the receiver account, and appends two `AccTransaction` rows — all committed atomically.

#### Scenario: Transfer debits sender and credits receiver
- **WHEN** an authorized user transfers 300 from Account A (Balance=1000) to Account B (Balance=500)
- **THEN** Account A Balance becomes 700, Account B Balance becomes 800, and two AccTransaction rows are appended

#### Scenario: Transfer cannot proceed if accounts are the same
- **WHEN** an authorized user attempts to transfer from an account to itself
- **THEN** the request is rejected with a validation error

### Requirement: AccTransaction is an append-only audit log
`AccTransaction` rows SHALL NOT be editable or directly deletable. Every balance-changing operation (Deposit, Expense, Transfer, and payment recording from Sales/Purchasing) appends a transaction row. Reversals append a new row with negative amounts rather than deleting the original.

#### Scenario: AccTransaction list is read-only
- **WHEN** an authorized user queries AccTransactions for an account
- **THEN** the full history is returned and no create/edit/delete endpoint exists for AccTransaction directly

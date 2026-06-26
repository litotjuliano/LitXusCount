namespace LitXusCount.Application.Accounts;

public record AccExpenseDto(
    long Id,
    long AccAccountId,
    string AccountName,
    string Name,
    DateTime ExpenseDate,
    decimal Amount,
    string? Note,
    bool IsActive);

public record AccExpenseCreateDto(
    long AccAccountId,
    string Name,
    DateTime ExpenseDate,
    decimal Amount,
    string? Note);

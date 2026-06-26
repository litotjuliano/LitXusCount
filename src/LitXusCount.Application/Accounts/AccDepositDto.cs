namespace LitXusCount.Application.Accounts;

public record AccDepositDto(
    long Id,
    long AccAccountId,
    string AccountName,
    DateTime DepositDate,
    decimal Amount,
    string? Note,
    bool IsActive);

public record AccDepositCreateDto(
    long AccAccountId,
    DateTime DepositDate,
    decimal Amount,
    string? Note);

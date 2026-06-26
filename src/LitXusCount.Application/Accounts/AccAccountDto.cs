namespace LitXusCount.Application.Accounts;

public record AccAccountDto(
    long Id,
    string Code,
    string AccountName,
    string? AccountNumber,
    string? Description,
    decimal Credit,
    decimal Debit,
    decimal Balance,
    bool IsActive);

public record AccAccountUpsertDto(
    string Code,
    string AccountName,
    string? AccountNumber,
    string? Description);

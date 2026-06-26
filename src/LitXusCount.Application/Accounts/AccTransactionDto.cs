using LitXusCount.Domain.Enums;

namespace LitXusCount.Application.Accounts;

public record AccTransactionDto(
    long Id,
    long AccAccountId,
    AccTransactionType Type,
    string TypeLabel,
    decimal Credit,
    decimal Debit,
    decimal Amount,
    string? Reference,
    string? Description,
    DateTime CreatedAt);

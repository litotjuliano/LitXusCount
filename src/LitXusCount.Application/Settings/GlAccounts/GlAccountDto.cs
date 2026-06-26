using LitXusCount.Domain.Entities;

namespace LitXusCount.Application.Settings.GlAccounts;

public record GlAccountDto(
    long Id,
    string Code,
    string Name,
    GlAccountType AccountType,
    long? ParentId,
    string? ParentName,
    bool IsControl,
    decimal OpeningBalance,
    bool IsActive);

public record GlAccountUpsertDto(
    string Code,
    string Name,
    GlAccountType AccountType,
    long? ParentId,
    bool IsControl,
    decimal OpeningBalance);

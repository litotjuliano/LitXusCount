namespace LitXusCount.Application.Settings.Currencies;

public record CurrencyDto(long Id, string Name, string Code, string? Symbol, string? Country, string? Description, bool IsDefault, bool IsActive);

public record CurrencyUpsertDto(string Name, string Code, string? Symbol, string? Country, string? Description);

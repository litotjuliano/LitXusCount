namespace LitXusCount.Application.Settings.VatPercentages;

public record VatPercentageDto(long Id, string Name, decimal Percentage, bool IsDefault, bool IsActive);

public record VatPercentageUpsertDto(string Name, decimal Percentage);

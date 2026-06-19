namespace LitXusCount.Application.Settings.VatPercentages;

public record VatPercentageDto(long Id, string Name, double Percentage, bool IsDefault, bool IsActive);

public record VatPercentageUpsertDto(string Name, double Percentage);

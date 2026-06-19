namespace LitXusCount.Application.Settings.Lookups;

public record LookupItemDto(long Id, string Name, string? Description, bool IsActive);

public record LookupItemUpsertDto(string Name, string? Description);

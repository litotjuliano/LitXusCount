namespace LitXusCount.Application.Admin.Roles;

public record RoleListItemDto(string Id, string Name, bool IsProtected);

public record RoleDetailDto(string Id, string Name, bool IsProtected, IReadOnlyList<string> Permissions);

public record RoleUpsertDto(string Name, IReadOnlyList<string> Permissions);

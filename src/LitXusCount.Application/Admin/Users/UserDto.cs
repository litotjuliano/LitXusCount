namespace LitXusCount.Application.Admin.Users;

public record UserDto(string Id, string Email, string? DisplayName, IReadOnlyList<string> Roles, bool IsActive);

public record UserCreateDto(string Email, string DisplayName, string Password, IReadOnlyList<string> Roles);

public record UserEditDto(string Email, string DisplayName, IReadOnlyList<string> Roles);

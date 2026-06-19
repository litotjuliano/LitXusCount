namespace LitXusCount.Application.Auth;

public class AuthResult
{
    public required string UserId { get; init; }
    public required string UserName { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
    public required string AccessToken { get; init; }
    public required DateTime AccessTokenExpiresAt { get; init; }
    public required string RefreshToken { get; init; }
}

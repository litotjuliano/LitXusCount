namespace LitXusCount.Application.Auth;

public interface IAuthService
{
    Task<AuthResult?> LoginAsync(string userNameOrEmail, string password, CancellationToken ct = default);
    Task<AuthResult?> RefreshAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Always succeeds from the caller's perspective (does not reveal whether the email exists).
    /// Returns a dev-only reset link when one was generated, for use before a real email sender exists.
    /// </summary>
    Task<string?> RequestPasswordResetAsync(string email, CancellationToken ct = default);

    Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default);
}

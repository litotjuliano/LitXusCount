using LitXusCount.Application.Settings.EmailConfigs;
using Microsoft.AspNetCore.DataProtection;

namespace LitXusCount.Infrastructure.Settings.EmailConfigs;

internal sealed class EmailConfigEncryptor : IEmailConfigEncryptor
{
    private const string Purpose = "LitXusCount.EmailConfig.Password";

    private readonly IDataProtector _protector;

    public EmailConfigEncryptor(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector(Purpose);
    }

    public string Encrypt(string plaintextPassword) => _protector.Protect(plaintextPassword);
}

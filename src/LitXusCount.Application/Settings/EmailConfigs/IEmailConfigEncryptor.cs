namespace LitXusCount.Application.Settings.EmailConfigs;

public interface IEmailConfigEncryptor
{
    string Encrypt(string plaintextPassword);
}

using System.Security.Cryptography;
using System.Text;

namespace DeploySystem.Api.Services;

public interface IPasswordService
{
    string Hash(string senha);
    bool Verificar(string senha, string hash);
}

public class PasswordService : IPasswordService
{
    public string Hash(string senha)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(senha + "DeploySystemSalt");
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool Verificar(string senha, string hash)
        => Hash(senha) == hash;
}

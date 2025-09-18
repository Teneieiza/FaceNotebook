using Konscious.Security.Cryptography;
using System.Text;

namespace FaceNoteBook.Utils;

public class PasswordHasher : IPasswordHasher
{
    public async Task<string> HashPasswordAsync(string password)
    {
        var salt = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var argon2 = new Argon2id(passwordBytes)
        {
            Salt = salt,
            DegreeOfParallelism = 8,
            MemorySize = 65536,
            Iterations = 4
        };

        var hash = await argon2.GetBytesAsync(32);

        var combined = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);

        return Convert.ToBase64String(combined);
    }

    public async Task<bool> VerifyHashedPasswordAsync(string password, string hashedPassword)
    {
        try
        {
            var combined = Convert.FromBase64String(hashedPassword);

            var salt = new byte[32];
            var hash = new byte[32];
            Buffer.BlockCopy(combined, 0, salt, 0, 32);
            Buffer.BlockCopy(combined, 32, hash, 0, 32);

            var passwordBytes = Encoding.UTF8.GetBytes(password);

            using var argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = 8,
                MemorySize = 65536,
                Iterations = 4
            };

            var newHash = await argon2.GetBytesAsync(32);
            return hash.SequenceEqual(newHash);
        }
        catch
        {
            return false;
        }
    }
}

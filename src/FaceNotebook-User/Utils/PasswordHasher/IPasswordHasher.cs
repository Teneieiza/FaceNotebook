namespace FaceNoteBook.Utils;

public interface IPasswordHasher
{
    Task<string> HashPasswordAsync(string password);
    Task<bool> VerifyHashedPasswordAsync(string password, string hashedPassword);
}

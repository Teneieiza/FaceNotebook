using FaceNoteBook.Models;

namespace FaceNoteBook.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}

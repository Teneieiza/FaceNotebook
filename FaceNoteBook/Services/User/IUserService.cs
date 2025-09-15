using FaceNoteBook.DTOs;

namespace FaceNoteBook.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto?> GetUserByIdAsync(Guid id);
    Task<UserResponseDto?> GetUserByEmailAsync(string email);
    Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> UserExistsAsync(Guid id);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> VerifyPasswordAsync(string email, string password);
}
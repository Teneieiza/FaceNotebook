using Microsoft.EntityFrameworkCore;
using FaceNoteBook.Data;
using FaceNoteBook.Models;
using FaceNoteBook.DTOs;
using FaceNoteBook.Utils;
using System.Text.RegularExpressions;

namespace FaceNoteBook.Services;

public class UserService : IUserService
{
    private readonly UserDataContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(UserDataContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    //------------------------------CRUD-------------------------------------

    // GET All users
    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();

        return users.Select(MapToResponseDto);
    }

    // GET users by ID
    public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        return user == null ? null : MapToResponseDto(user);
    }

    // GET users by Email
    public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        return user == null ? null : MapToResponseDto(user);
    }

    // Create new user
    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        if (await EmailExistsAsync(createUserDto.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$");
        if (!passwordRegex.IsMatch(createUserDto.Password))
        {
            throw new InvalidOperationException(
                "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, and one number."
            );
        }

        var hashedPassword = await _passwordHasher.HashPasswordAsync(createUserDto.Password);

        var user = new User
        {
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            Password = hashedPassword,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
            UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToResponseDto(user);
    }

    // Update user details
    public async Task<UserResponseDto?> UpdateDetailAsync(Guid id, UpdateDetailDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        user.Name = dto.Name;
        user.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _context.SaveChangesAsync();
        return MapToResponseDto(user);
    }

    // Update user email
    public async Task<UserResponseDto?> UpdateEmailAsync(Guid id, UpdateEmailDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        if (await EmailExistsAsync(dto.Email) && dto.Email != user.Email)
        {
            throw new InvalidOperationException("Email already exists");
        }

        user.Email = dto.Email;
        user.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _context.SaveChangesAsync();
        return MapToResponseDto(user);
    }

    // Update user password
    public async Task<UserResponseDto?> UpdatePasswordAsync(Guid id, UpdatePasswordDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$");
        if (!passwordRegex.IsMatch(dto.NewPassword))
        {
            throw new InvalidOperationException(
                "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, and one number."
            );
        }

        var oldPasswordValid = await _passwordHasher.VerifyHashedPasswordAsync(dto.OldPassword, user.Password);
        if (!oldPasswordValid)
        {
            throw new InvalidOperationException("Old password is incorrect");
        }

        var newPasswordSameAsOld = await _passwordHasher.VerifyHashedPasswordAsync(dto.NewPassword, user.Password);
        if (newPasswordSameAsOld)
        {
            throw new InvalidOperationException("New password cannot be the same as the old password");
        }

        user.Password = await _passwordHasher.HashPasswordAsync(dto.NewPassword);
        user.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _context.SaveChangesAsync();
        return MapToResponseDto(user);
    }

    // Delete user
    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }



    //------------------------------Checking-------------------------------------

    // Check user exists by ID
    public async Task<bool> UserExistsAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    // Check user exists by Email
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    // Verify user password
    public async Task<bool> VerifyPasswordAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            throw new InvalidOperationException("Password incorrect or user not found");
        }

        return await _passwordHasher.VerifyHashedPasswordAsync(password, user.Password);
    }

    private static UserResponseDto MapToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
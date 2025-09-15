using Microsoft.EntityFrameworkCore;
using FaceNoteBook.Data;
using FaceNoteBook.Models;
using FaceNoteBook.DTOs;
using Konscious.Security.Cryptography;
using System.Text;

namespace FaceNoteBook.Services;

public class UserService : IUserService
{
    private readonly UserDataContext _context;

    public UserService(UserDataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();

        return users.Select(MapToResponseDto);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        return user == null ? null : MapToResponseDto(user);
    }

    public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
        
        return user == null ? null : MapToResponseDto(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        if (await EmailExistsAsync(createUserDto.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        var hashedPassword = await HashPasswordAsync(createUserDto.Password);

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

    public async Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(updateUserDto.Email) && 
            updateUserDto.Email != user.Email && 
            await EmailExistsAsync(updateUserDto.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        if (!string.IsNullOrEmpty(updateUserDto.Name))
        {
            user.Name = updateUserDto.Name;
        }

        if (!string.IsNullOrEmpty(updateUserDto.Email))
        {
            user.Email = updateUserDto.Email;
        }

        if (!string.IsNullOrEmpty(updateUserDto.Password))
        {
            user.Password = await HashPasswordAsync(updateUserDto.Password);
        }

        user.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _context.SaveChangesAsync();
        return MapToResponseDto(user);
    }

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

    public async Task<bool> UserExistsAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> VerifyPasswordAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false;
        }

        return await VerifyHashedPasswordAsync(password, user.Password);
    }

    private static async Task<string> HashPasswordAsync(string password)
    {
        var salt = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

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

        // แปลงเป็น Base64 string
        return Convert.ToBase64String(combined);
    }

    private static async Task<bool> VerifyHashedPasswordAsync(string password, string hashedPassword)
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
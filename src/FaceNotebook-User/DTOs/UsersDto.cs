using System.ComponentModel.DataAnnotations;

namespace FaceNoteBook.DTOs;

public class CreateUserDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(150)]
    public string Password { get; set; } = string.Empty;
}

public class UpdateDetailDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateEmailDto
{
    public string Email { get; set; } = string.Empty;
}

public class UpdatePasswordDto
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class UserLoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse(int status, string message, T? data = default)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }
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

public class UpdateUserDto
{
    [StringLength(50)]
    public string? Name { get; set; }

    [StringLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(150)]
    public string? Password { get; set; }
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
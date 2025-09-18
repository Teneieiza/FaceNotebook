using System.ComponentModel.DataAnnotations;

namespace FaceNoteBook.Models;

public class User
{
  public Guid Id { get; set; }

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

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
  
  public string? RefreshToken { get; set; }
  public DateTime? RefreshTokenExpiryTime { get; set; }

}
using System.ComponentModel.DataAnnotations;

namespace DemoAPINET.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    [Required]
    [MaxLength(128)]
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    [MaxLength(50)]
    public string Role { get; set; } = "Employee";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;
public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }=string.Empty;
    [Required]
    public string DisplayName { get; set; }=string.Empty;
    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{4,}$", ErrorMessage = "Password must be complex and contain at least 1 uppercase, 1 lowercase, 1 number, 1 non-alphanumeric character, and be at least 4 characters long.")]
    public string Password { get; set; }=string.Empty;
}
using System.ComponentModel.DataAnnotations;
using AGAMinigameApi.Validations;

namespace AGAMinigameApi.Dtos.Auth;

public class RegisterRequestDto
{
    [Required]
    [StringLength(64, MinimumLength = 4, ErrorMessage = "Account must be between 4 and 64 characters.")]
    public string Account { get; set; } = string.Empty;

    [Required]
    [StringLength(64, MinimumLength = 4, ErrorMessage = "Nickname must be between 4 and 64 characters.")]
    public string Nickname { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [MinimumAge(18, ErrorMessage = "You must be 18 years or older.")]
    public DateTime Dob { get; set; }


    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
using System.ComponentModel.DataAnnotations;

namespace AGAMinigameApi.Dtos.Auth;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}
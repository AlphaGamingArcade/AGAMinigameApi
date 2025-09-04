using System.ComponentModel.DataAnnotations;

namespace AGAMinigameApi.Dtos.Auth;

public class ResendVerifyEmailDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

}
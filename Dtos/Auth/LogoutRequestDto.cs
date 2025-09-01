using System.ComponentModel.DataAnnotations;

namespace AGAMinigameApi.Dtos.Auth;

public class LogoutRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
using System.ComponentModel.DataAnnotations;

namespace AGAMinigameApi.Dtos.Auth;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
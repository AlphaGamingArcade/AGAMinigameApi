namespace AGAMinigameApi.Dtos.Auth;

public class ForgotPasswordDto
{
    public int MemberId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConsumedAt { get; set; }
}
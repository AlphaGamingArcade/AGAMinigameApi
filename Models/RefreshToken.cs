namespace AGAMinigameApi.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}
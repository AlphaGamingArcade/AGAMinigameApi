namespace AGAMinigameApi.Models;

public static class VerificationPurposes
{
    public const string EmailVerification = "email_verification";
    public const string PasswordReset = "password_reset";
    public const string TwoFactor = "two_factor";
    public const string ChangeEmail = "change_email";
}

public class EmailVerification
{
    public long Id { get; set; }
    public int MemberId { get; set; }
    public string Email { get; set; } = "";
    public string? AppKey { get; set; }
    public string Token { get; set; } = ""; 
    public string Purpose { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? ConsumedAtUtc { get; set; }
}

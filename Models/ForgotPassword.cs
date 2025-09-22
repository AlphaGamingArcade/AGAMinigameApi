namespace AGAMinigameApi.Models
{
    public class ForgotPassword
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public char IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
    }
}
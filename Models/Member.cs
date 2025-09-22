namespace AGAMinigameApi.Models
{
    public class Member
    {
        public int Id { get; set; }
        public short AgentId { get; set; }
        public string Account { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Gamemoney { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
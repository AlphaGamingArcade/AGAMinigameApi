namespace AGAMinigameApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public DateTime Dob { get; set; }
        public char EmailStatus { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
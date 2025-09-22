namespace AGAMinigameApi.Dtos.Member
{
    public class MemberDto
    {
        public int Id { get; set; }
        public short AgentId { get; set; }
        public string? Account { get; set; }
        public string? Nickname { get; set; }
        public string? Email { get; set; }
        public decimal Gamemoney { get; set; }
        public string? Currency { get; set; }
        public string? Token { get; set; }
        public DateTime? NicknameUpdate { get; set; }
    }
}
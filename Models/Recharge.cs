namespace AGAMinigameApi.Models
{
    public class Recharge
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public short AgentId { get; set; }
        public decimal Gamemoney { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public DateTime Datetime { get; set; }
    }
}
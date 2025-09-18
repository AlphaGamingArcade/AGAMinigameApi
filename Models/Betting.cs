namespace AGAMinigameApi.Models
{
    public class Betting
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public int GamecodeId { get; set; }
        public short AgentId { get; set; }
        public decimal Money { get; set; }
        public decimal Benefit { get; set; }
        public char Result { get; set; }
        public DateTime Datetime { get; set; }
        
        // extra
        public Gamecode? Gamecode { get; set; }
    }
}
namespace AGAMinigameApi.Dtos.Banner
{
    public class BettingDto
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public int GamecodeId { get; set; }
        public string GamecodeName { get; set; } = string.Empty;
        public decimal Money { get; set; }
        public decimal Benefit { get; set; }
        public char Result { get; set; }
        public DateTime Datetime { get; set; }
    }
}
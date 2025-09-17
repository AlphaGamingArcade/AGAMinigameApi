namespace AGAMinigameApi.Dtos.Charge
{
    public class ChargeDto
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public decimal Gamemoney { get; set; }
        public string? Currency { get; set; }
        public DateOnly Date { get; set; }
        public DateTime Datetime { get; set; }
    }
}
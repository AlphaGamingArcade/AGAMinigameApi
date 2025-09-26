namespace AGAMinigameApi.Dtos.Charge
{
    public class FreeChargeClaimDto
    {
        public bool Claimed { get; set; }
        public int Amount { get; set; }
        public DateTime? NextClaimDateUtc { get; set; }
        public int SecondsUntilNextClaim { get; set; }
    }
}
namespace AGAMinigameApi.Dtos.Auth
{
    public class EmailStatusResponseDto
    {
        public bool IsVerified { get; set; }
        public DateTime? Datetime { get; set; }
    }
}
namespace AGAMinigameApi.Models
{
    public class Play
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public int GameId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
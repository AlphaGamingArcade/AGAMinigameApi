namespace AGAMinigameApi.Models
{
    public class Favorite
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public int GameId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // extra
        public Game? Game { get; set; }
    }
}
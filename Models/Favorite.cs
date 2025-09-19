namespace AGAMinigameApi.Models
{
    public class Favorite
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public int GameId { get; set; }
        public string GameType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
namespace AGAMinigameApi.Dtos.Favorite
{
    public class FavoriteDto
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public int GameId { get; set; }
        public string? GameType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
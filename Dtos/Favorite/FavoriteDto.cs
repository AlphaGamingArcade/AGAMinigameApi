namespace AGAMinigameApi.Dtos.Favorite
{
    public class FavoriteDto
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public string? ItemType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
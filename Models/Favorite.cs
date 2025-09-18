namespace AGAMinigameApi.Models
{
    public class Favorite
    {
        public long Id { get; set; }
        public int MemberId { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
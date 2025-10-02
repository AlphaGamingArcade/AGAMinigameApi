namespace AGAMinigameApi.Models
{
    public class GamePreview
    {
        public int Id { get; set; }
        public short GameId { get; set; }
        public string Image { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public DateTime Datetime { get; set; }
    }
}
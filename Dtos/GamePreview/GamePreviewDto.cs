namespace AGAMinigameApi.Dtos.GamePreview
{
    public class GamePreviewDto
    {
        public int Id { get; set; }
        public short GameId { get; set; } 
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Datetime { get; set; }
    }
}
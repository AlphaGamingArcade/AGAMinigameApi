using AGAMinigameApi.Dtos.Gamecode;
using AGAMinigameApi.Dtos.GamePreview;

namespace AGAMinigameApi.Dtos.Game
{
    public class GameDto
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public object DescriptionMultiLanguage { get; set; } = new();
        public string? Image { get; set; }
        public string? PlayUrl { get; set; }
        public char Status { get; set; }
        public char Top { get; set; }
        public char Trending { get; set; }
        public DateTime Datetime { get; set; }
        public GamecodeDto? Gamecode { get; set; }
    }

    // Extended DTO for details
    public class GameDetailDto : GameDto
    {
        public List<GamePreviewDto> GamePreviews { get; set; } = new();
        public int? TotalPlayers { get; set; }
        // public int ActivePlayers { get; set; }
        // public double AverageRating { get; set; }
        // public int TotalReviews { get; set; }
    }
}
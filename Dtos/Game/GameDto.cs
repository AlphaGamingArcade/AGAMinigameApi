namespace AGAMinigameApi.Dtos.Banner
{
    public class GameDto
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public object DescriptionMultiLanguage { get; set; } = new();
        public string? Image { get; set; }
        public string? Url { get; set; }
        public char Status { get; set; }
        public char Top { get; set; }
        public char Trending { get; set; }
        public DateTime Datetime { get; set; }
        public GamecodeDto? Gamecode { get; set; }
    }
}
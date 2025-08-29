namespace AGAMinigameApi.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public char Status { get; set; }
        public char Top { get; set; }
        public char Trending { get; set; }
        public DateTime Datetime { get; set; }
    }
}
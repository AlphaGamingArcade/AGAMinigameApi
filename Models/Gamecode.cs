namespace AGAMinigameApi.Models
{
    public class Gamecode
    {
        public byte Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameMultiLanguage { get; set; } = string.Empty;
        public double Percent { get; set; }
        public DateTime Datetime { get; set; }
        public string Status { get; set; } = string.Empty;
        public byte Order { get; set; }
        public string GameType { get; set; } = string.Empty;
    }
}

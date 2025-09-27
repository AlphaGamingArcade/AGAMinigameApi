namespace AGAMinigameApi.Models
{
    public class Play
    {
        public int MemberId { get; set; }
        public string Gamecode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // extra
        public Game? Game { get; set; }
    }
}
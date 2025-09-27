namespace AGAMinigameApi.Dtos.Play
{
    public class PlayDto
    {
        public long Id { get; set; } 
        public int MemberId { get; set; }
        public int GameId { get; set; }
        public string? PlayUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
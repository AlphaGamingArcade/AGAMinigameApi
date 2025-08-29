namespace AGAMinigameApi.Dtos.Banner
{
    public class GameDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Url { get; set; }
        public char Status { get; set; }
        public char Top { get; set; }
        public char Trending { get; set; }
        public DateTime Datetime { get; set; }
    }
}
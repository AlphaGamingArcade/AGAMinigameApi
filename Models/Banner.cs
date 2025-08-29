namespace AGAMinigameApi.Models
{
    public class Banner
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Url { get; set; }
        public int Order { get; set; }
        public DateTime Datetime { get; set; }
    }
}
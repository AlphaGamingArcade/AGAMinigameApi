namespace AGAMinigameApi.Dtos.Banner
{
    public class CreateBannerDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Url { get; set; }
        public int Order { get; set; }
    }
}
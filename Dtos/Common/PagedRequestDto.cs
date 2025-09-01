namespace AGAMinigameApi.Dtos.Common
{
    public class PagedRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Optional: sorting
        public string? SortBy { get; set; }
        public bool Descending { get; set; } = false;
    }
}
namespace AGAMinigameApi.Dtos.Common
{
    public class DateFilteredPagedRequestDto : PagedRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
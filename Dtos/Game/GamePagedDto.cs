
using AGAMinigameApi.Dtos.Common;

namespace AGAMinigameApi.Dtos.Banner
{
    public class GamePagedRequestDto : PagedRequestDto
    {
        public bool? Top { get; set; }       // ?top=true
        public bool? Trending { get; set; }  // ?trending=true
        public bool? Latest { get; set; }    // ?latest=true
    }
}
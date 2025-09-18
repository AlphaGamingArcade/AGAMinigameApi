using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("bettings")]
public class BettingController : ControllerBase
{
    private readonly ILogger<BettingController> _logger;
    private readonly IBannerService _bannerService;

    public BettingController(ILogger<BettingController> logger, IBannerService bannerService)
    {
        _logger = logger;
        _bannerService = bannerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedBettings([FromQuery] PagedRequestDto requestDto)
    {
        var result = await _bannerService.GetPaginatedBannersAsync(requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }
}

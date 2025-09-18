using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("favorites")]
public class FavoriteController : ControllerBase
{
    private readonly ILogger<FavoriteController> _logger;
    private readonly IBannerService _bannerService;

    public FavoriteController(ILogger<FavoriteController> logger, IBannerService bannerService)
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

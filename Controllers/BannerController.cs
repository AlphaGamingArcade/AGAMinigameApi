using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("banners")]
public class BannerController : ControllerBase
{
    private readonly ILogger<BannerController> _logger;
    private readonly IBannerService _bannerService;

    public BannerController(ILogger<BannerController> logger, IBannerService bannerService)
    {
        _logger = logger;
        _bannerService = bannerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedBanners([FromQuery] PagedRequestDto requestDto)
    {
        var result = await _bannerService.GetPaginatedBannersAsync(requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }
}

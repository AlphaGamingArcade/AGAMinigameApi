using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Repositories;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("banners")]
public class BannerController : ControllerBase
{
    private readonly ILogger<BannerController> _logger;
    private readonly IBannerRepository _bannerRepository;

    public BannerController(ILogger<BannerController> logger, IBannerRepository bannerRepository)
    {
        _logger = logger;
        _bannerRepository = bannerRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<BannerDto>> Get()
    {
        var banners = await _bannerRepository.GetAll();
        return banners.Select(b => b.ToBannerDto());
    }
}

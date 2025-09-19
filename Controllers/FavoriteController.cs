using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("favorites")]
public class FavoriteController : ControllerBase
{
    private readonly ILogger<FavoriteController> _logger;
    private readonly IFavoriteService _favoriteService;

    public FavoriteController(ILogger<FavoriteController> logger, IFavoriteService favoriteService)
    {
        _logger = logger;
        _favoriteService = favoriteService;
    }
}

using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("bettings")]
public class BettingController : ControllerBase
{
    private readonly ILogger<BettingController> _logger;
    private readonly IBettingService _bettingService;

    public BettingController(ILogger<BettingController> logger, BettingService bettingService)
    {
        _logger = logger;
        _bettingService = bettingService;
    }
}

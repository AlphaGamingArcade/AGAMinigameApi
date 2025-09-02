using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("recharges")]
public class RechargeController : ControllerBase
{
    private readonly ILogger<RechargeController> _logger;
    private readonly IGameService _gameService;

    public RechargeController(ILogger<RechargeController> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedRecharges([FromQuery] PagedRequestDto requestDto)
    {
        var result = await _gameService.GetPaginatedGamesAsync(requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }
}

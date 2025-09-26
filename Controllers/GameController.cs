using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("games")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly IGameService _gameService;

    public GameController(ILogger<GameController> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginatedGames([FromQuery] PagedRequestDto requestDto)
    {
        var result = await _gameService.GetPaginatedGamesAsync(requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpGet("top")]
    public async Task<IActionResult> GetTopPaginatedGames([FromQuery] PagedRequestDto requestDto)
    {
        var result = await _gameService.GetTopPaginatedGamesAsync(requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpGet("trending")]
    public async Task<IActionResult> GetTrendingPaginatedGames([FromQuery] PagedRequestDto requestDto)
    {
        var result = await _gameService.GetTrendingPaginatedGamesAsync(requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestPaginatedGames([FromQuery] PagedRequestDto requestDto)
    {
        var result = await _gameService.GetLatestPaginatedGamesAsync(requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [Authorize]
    [HttpGet("{gameId:int}/play")]
    public async Task<IActionResult> GetPlay(int gameId)
    {
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "Success", "this will be link", 200));
    }
}

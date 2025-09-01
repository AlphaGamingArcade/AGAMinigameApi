using AGAMinigameApi.Dtos;
using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Interfaces;
using AGAMinigameApi.Services;
using api.Mappers;
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
        return Ok(new ApiResponse<object>(true, "Sucess", result, 200));
    }
}

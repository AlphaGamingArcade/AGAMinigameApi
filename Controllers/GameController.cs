using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("games")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly IGameRepository _gameRepository;

    public GameController(ILogger<GameController> logger, IGameRepository gameRepository)
    {
        _logger = logger;
        _gameRepository = gameRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<GameDto>> Get()
    {
        var games = await _gameRepository.GetAll();
        return games.Select(g => g.ToGameDto());
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Dtos.Favorite;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostFavorite([FromBody] CreateFavoriteDto createDto)
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(sub))
            return Unauthorized(new ApiResponse<object>(false, "Missing sub claim", null, 401));

        int memberId = int.Parse(sub);
        var result = await _favoriteService.CreateMemberFavoriteDto(memberId, createDto);

        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }
}

using AGAMinigameApi.Dtos.Charge;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Dtos.Favorite;
using AGAMinigameApi.Dtos.Member;
using AGAMinigameApi.Dtos.Play;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;
  
[ApiController]
[Route("members")]
public class MemberController : ControllerBase
{
    private readonly ILogger<MemberController> _logger;
    private readonly IChargeService _chargeService;
    private readonly IMemberService _memberService;
    private readonly IBettingService _bettingService;
    private readonly IFavoriteService _favoriteService;
    private readonly IGameService _gameService;

    public MemberController(ILogger<MemberController> logger, IChargeService chargeService, IMemberService memberService, IBettingService bettingService, IFavoriteService favoriteService, IGameService gameService)
    {
        _logger = logger;
        _chargeService = chargeService;
        _memberService = memberService;
        _bettingService = bettingService;
        _favoriteService = favoriteService;
        _gameService = gameService;
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetMember(int id)
    {
        var result = await _memberService.GetMemberByIdAsync(id);
        if (result == null)
            return NotFound(new ApiResponse<object>(false, "Member not found", null, 404));
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpPatch("{id:int}")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> PatchMember(int id, [FromBody] PatchMemberDto request)
    {
        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound(new ApiResponse<object>(false, "Member not found", null, 404));

        if (string.Equals(member.Nickname, request.Nickname, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ApiResponse<object>(
                false,
                "NicknameUnchanged",
                "New nickname must be different from the current nickname.",
                400));
        }

        if (member.NicknameUpdate.HasValue)
        {
            var lastUpdate = member.NicknameUpdate.Value;
            var nextAllowedAt = lastUpdate.AddDays(30);

            if (DateTime.UtcNow < nextAllowedAt)
            {
                return StatusCode(429, new ApiResponse<object>(
                    false,
                    "Nickname change cooldown",
                    new { nextAllowedAt }, 429));
            }
        }

        await _memberService.UpdateMemberNicknameAsync(id, request.Nickname!);

        var refreshed = await _memberService.GetMemberByIdAsync(id);

        return Ok(new ApiResponse<object>(true, "Success", refreshed, 200));
    }

    [HttpGet("{id:int}/charges")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetPaginatedMemberCharges(int id, [FromQuery] PagedRequestDto requestDto)
    {
        var result = await _chargeService.GetPaginatedMemberChargesAsync(id, requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpGet("{id:int}/charges/free-claim")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetMemberChargeFreeClaim(int id)
    {
        var nowUtc = DateHelper.GetUtcNow();
        var dto = await _chargeService.GetFreeClaimStatusAsync(id, nowUtc);
        var message = dto.Claimed ? "Already claimed" : "Recharge available";
        return Ok(new ApiResponse<FreeChargeClaimDto>(true, message, dto, 200));
    }

    [HttpPost("{id:int}/charges/free-claim")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> CreateMemberCharge(int id)
    {
        var now = DateHelper.GetUtcNow();
        var amount = 100000;

        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
            return NotFound(new ApiResponse<object>(false, "Member not found", null, 404));

        if (await _chargeService.IsChargeExistsAsync(id, now))
        {
            return Conflict(new ApiResponse<object>(
                false,
                "Failed",
                "Charge already exists for today.",
                409
            ));
        }

        var charge = await _chargeService.ChargeMemberAsync(member, now, amount);

        return Ok(new ApiResponse<object>(true, "Success", charge, 200));
    }


    [HttpGet("{id:int}/bettings")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetPaginatedMemberBettings(int id, [FromQuery] PagedRequestDto requestDto)
    {
        var result = await _bettingService.GetPaginatedMemberBettingsAsync(id, requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpGet("{id:int}/plays")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetPaginatedMemberPlays(int id, [FromQuery] PagedRequestDto requestDto)
    {
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "Success", "Paginated recent games", 200));
    }
    
    [HttpPost("{id:int}/plays")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> CreateMemberPlay(int id, [FromBody] CreatePlayRequestDto requestDto)
    {
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "Success", "Played successfully", 200));
    }
    
    [HttpGet("{id:int}/favorites")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetPaginatedMemberFavorites(int id, [FromQuery] PagedRequestDto requestDto)
    {
        var result = await _favoriteService.GetPaginatedMemberFavoritesAsync(id, requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpPost("{id:int}/favorites")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> CreateMemberFavorite(int id, [FromBody] CreateFavoriteDto createDto)
    {
        var game = await _gameService.GetGameAsync(createDto.GameId);
        if (game == null)
            return NotFound(new ApiResponse<object>(false, "Failed", "Game not found", 404));

        if (await _favoriteService.IsMemberFavoriteExistsAsync(id, createDto.GameId))
        {
            return Conflict(new ApiResponse<object>(false, "Failed", "Favorite already added", 409));
        }

        var result = await _favoriteService.CreateMemberFavoriteAsync(id, createDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpDelete("{id:int}/favorites/{gameId:int}")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> DeleteMemberFavorite(int id, int gameId)
    {
        if (!await _favoriteService.IsMemberFavoriteExistsAsync(id, gameId))
        {
            return NotFound(new ApiResponse<object>(
                false, "Failed", "Favorite not found", 404));
        }
        var result = await _favoriteService.DeleteMemberFavoriteAsync(id, gameId);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }
}

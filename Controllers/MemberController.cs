using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("members")]
public class MemberController : ControllerBase
{
    private readonly ILogger<MemberController> _logger;
    private readonly IRechargeService _rechargeService;

    public MemberController(ILogger<MemberController> logger, IRechargeService rechargeService)
    {
        _logger = logger;
        _rechargeService = rechargeService;
    }


    [HttpGet("/{memberId:int}")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetMember(int memberId, [FromQuery] PagedRequestDto requestDto)
    {
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "Success", memberId, 200));
    }

    [HttpGet("/{memberId:int}/recharges")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetPaginatedMemberRecharges(int memberId, [FromQuery] PagedRequestDto requestDto)
    {
        var result = await _rechargeService.GetPaginatedMemberRechargesAsync(memberId, requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }
}

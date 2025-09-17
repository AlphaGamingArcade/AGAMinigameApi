using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Repositories;
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
    private readonly IMemberService _memberService;

    public MemberController(ILogger<MemberController> logger, IRechargeService rechargeService, IMemberService memberService)
    {
        _logger = logger;
        _rechargeService = rechargeService;
        _memberService = memberService;
    }

    [HttpGet("/{memberId:int}")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetMember(int memberId)
    {
        var result = await _memberService.GetMemberByIdAsync(memberId);
        if (result == null)
        {
            return NotFound(new ApiResponse<object>(false, "Member not found", null, 404));
        }
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpGet("/{memberId:int}/recharges")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetPaginatedMemberRecharges(int memberId, [FromQuery] PagedRequestDto requestDto)
    {
        var result = await _rechargeService.GetPaginatedMemberRechargesAsync(memberId, requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpPost("/{memberId:int}/recharges")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> PostMemberRecharge(int memberId)
    {
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "Success", memberId, 200));
    }
}

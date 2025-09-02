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

    [HttpGet("/{memberId:int}/recharges")]
    [Authorize]
    public async Task<IActionResult> GetPaginatedMemberRecharges(int memberId, [FromQuery] PagedRequestDto requestDto)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub); 
        var result = await _rechargeService.GetPaginatedMemberRechargesAsync(memberId, requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }
}

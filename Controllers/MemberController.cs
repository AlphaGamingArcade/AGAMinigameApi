using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations.Rules;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("members")]
public class MemberController : ControllerBase
{
    private readonly ILogger<MemberController> _logger;
    private readonly IChargeService _chargeService;
    private readonly IMemberService _memberService;
    private readonly IBettingService _bettingService;

    public MemberController(ILogger<MemberController> logger, IChargeService chargeService, IMemberService memberService, IBettingService bettingService)
    {
        _logger = logger;
        _chargeService = chargeService;
        _memberService = memberService;
        _bettingService = bettingService;
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetMember(int id)
    {
        var result = await _memberService.GetMemberByIdAsync(id);
        if (result == null)
        {
            return NotFound(new ApiResponse<object>(false, "Member not found", null, 404));
        }
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpGet("{id:int}/charges")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> GetPaginatedMemberCharges(int id, [FromQuery] PagedRequestDto requestDto)
    {
        var result = await _chargeService.GetPaginatedMemberChargesAsync(id, requestDto);
        return Ok(new ApiResponse<object>(true, "Success", result, 200));
    }

    [HttpPost("{id:int}/charges")]
    [Authorize(Policy = "OwnerOrAdmin")]
    public async Task<IActionResult> PostMemberCharge(int id)
    {
        var now = DateHelper.GetUtcNow();
        var amount = 100000;

        var member = await _memberService.GetMemberByIdAsync(id);
        if (member == null)
        {
            return NotFound(new ApiResponse<object>(false, "Member not found", null, 404));
        }

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
}

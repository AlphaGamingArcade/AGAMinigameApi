using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("recharges")]
[Authorize]
public class RechargeController : ControllerBase
{
    private readonly ILogger<RechargeController> _logger;
    private readonly IRechargeService _rechargeService;

    public RechargeController(ILogger<RechargeController> logger, IRechargeService rechargeService)
    {
        _logger = logger;
        _rechargeService = rechargeService;
    }
}

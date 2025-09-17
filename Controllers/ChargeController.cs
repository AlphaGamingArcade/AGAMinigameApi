using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("charges")]
[Authorize]
public class ChargeController : ControllerBase
{
    private readonly ILogger<ChargeController> _logger;
    private readonly IChargeService _chargeService;

    public ChargeController(ILogger<ChargeController> logger, IChargeService chargeService)
    {
        _logger = logger;
        _chargeService = chargeService;
    }
}

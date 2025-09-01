using AGAMinigameApi.Dtos.Auth;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Interfaces;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    // POST /auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        await Task.Delay(1000); 
        return Ok(new ApiResponse<object>(true, "User registration successful.", null, 200));
    }

    // POST /auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    { 
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "User login successful.", null, 200));
     }

    // POST /auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    { 
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "Logout successful.", null, 200));
     }

    // POST /auth/forgot-password
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    { 
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "Forgot password sent to email.", null, 200));
    }

    // POST /auth/reset-password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    { 
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "Reset password successfully.", null, 200));
     }

    // POST /auth/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
    { 
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "Token refreshed.", null, 200));
     }

    // GET /auth/confirm-email
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    { 
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "Email confirmed.", null, 200));
    }
}

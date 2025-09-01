using AGAMinigameApi.Dtos.Auth;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;
    private readonly IAgentService _agentService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService, IAgentService agentService, IRefreshTokenService refreshTokenService)
    {
        _logger = logger;
        _authService = authService;
        _agentService = agentService;
        _refreshTokenService = refreshTokenService;
    }

    // POST /auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var agent = await _agentService.GetAgentByCodeAsync("agaminigame");
        if (agent is null)
        {
            const int status = StatusCodes.Status404NotFound;
            return Conflict(new ApiResponse<object>(false, "AGA Minigame agent not found.", null, status));
        }

        if (await _authService.UserExistsByEmailAsync(request.Email))
        {
            const int status = StatusCodes.Status409Conflict;
            return Conflict(new ApiResponse<object>(false, "User email already in use.", null, status));
        }

        if (await _authService.UserExistsByAccountAsync(request.Account))
        {
            const int status = StatusCodes.Status409Conflict;
            return Conflict(new ApiResponse<object>(false, "User account already in use.", null, status));
        }

        var result = await _authService.RegisterAsync(request, agent);

        const int created = StatusCodes.Status201Created;
        return Ok(new ApiResponse<object>(true, "User registration successful.", result, created));
    }

    // POST /auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _authService.GetUserByEmailAsync(request.Email); 
        if (user is null)
        {
            const int status = StatusCodes.Status404NotFound;
            return Conflict(new ApiResponse<object>(false, "Email not registered.", null, status));
        }

        // TODO : Make this hashed
        if (user.Password != request.Password)
        {
            const int status = StatusCodes.Status401Unauthorized;
            return Conflict(new ApiResponse<object>(false, "Incorrect password.", null, status));
        }

        var result = await _authService.LoginAsync(request, user);
        return Ok(new ApiResponse<object>(true, "User login successful.", result, 200));
    }

    // POST /auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
    {
        var token = await _refreshTokenService.GetRefreshTokenByTokenAsync(request.RefreshToken);
        if (token is null)
        {
            const int status = StatusCodes.Status401Unauthorized;
            return Unauthorized(new ApiResponse<object>(false, "Invalid refresh token.", null, status));
        }

        await _authService.LogoutAsync(token.MemberId);

        return Ok(new ApiResponse<object>(true, "Logged out successfully.", null, 200));
        
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
        var token = await _refreshTokenService.GetRefreshTokenByTokenAsync(request.RefreshToken);
        if (token is null)
        {
            const int status = StatusCodes.Status401Unauthorized;
            return Unauthorized(new ApiResponse<object>(false, "Invalid refresh token.", null, status));
        }

        var result = await _authService.RefreshTokenAsync(token.MemberId);

        return Ok(new ApiResponse<object>(true, "Token refreshed.", result, 200));
    }

    // GET /auth/confirm-email
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    { 
        await Task.Delay(1000);
        return Ok(new ApiResponse<object>(true, "Email confirmed.", null, 200));
    }
}

using AGAMinigameApi.Dtos.Auth;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmptOptions;

namespace AGAMinigameApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;
    private readonly IAgentService _agentService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IEmailVerificationService _emailVerificationService;
    private readonly IForgotPasswordService _forgotPasswordService;
    private readonly IMemberService _memberService;
    private readonly IOptions<AppOptions> _appOptions;

    public AuthController(ILogger<AuthController> logger, IAuthService authService, IAgentService agentService, IRefreshTokenService refreshTokenService, IMemberService memberService, IEmailVerificationService emailVerificationService, IForgotPasswordService forgotPasswordService, IOptions<AppOptions> appOptions)
    {
        _logger = logger;
        _authService = authService;
        _agentService = agentService;
        _refreshTokenService = refreshTokenService;
        _emailVerificationService = emailVerificationService;
        _forgotPasswordService = forgotPasswordService;
        _memberService = memberService;
        _appOptions = appOptions;
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

        var (emailTaken, accountTaken) = await _authService.CheckUserConflictsAsync(request.Email, request.Account);

        if (emailTaken)
        {
            const int status = StatusCodes.Status409Conflict;
            return Conflict(new ApiResponse<object>(false, "User email already in use.", null, status));
        }

        if (accountTaken)
        {
            const int status = StatusCodes.Status409Conflict;
            return Conflict(new ApiResponse<object>(false, "User account already in use.", null, status));
        }

        await _authService.RegisterAsync(request, agent);

        const int created = StatusCodes.Status201Created;
        return Ok(new ApiResponse<object>(true, "Registration successful. Please check your email to verify your account.", null, created));
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

        if (user.EmailStatus != 'y')
        {
            const int status = StatusCodes.Status403Forbidden;
            return StatusCode(status, new ApiResponse<object>(false, "Email not verified.", null, status));
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
        var now = DateHelper.GetUtcNow();
        var member = await _memberService.GetMemberByEmailAsync(request.Email!);
        if (member == null)
        {
            await Task.Delay(2000); // trick hacker that it process something
            return Ok(new ApiResponse<object>(true, "Forgot password sent to email.", null, 200));
        }

        await _forgotPasswordService.SendLinkAsync(
            member.Id,
            member.Email!,
            member.Nickname!,
            now
        );
        
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
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        var now = DateHelper.GetUtcNow();
        string tokenHash = HashHelper.ComputeSHA256(token);
        var ok = await _emailVerificationService.VerifyAsync(tokenHash, now);
        if (ok)
        {
            await _authService.SetEmailVerifiedAsync(tokenHash, now);
        }

        var successUrl = $"{_appOptions.Value.Url.TrimEnd('/')}/verify/success.html";
        var failureUrl = $"{_appOptions.Value.Url.TrimEnd('/')}/verify/failure.html";
        return Redirect(ok ? successUrl : failureUrl);
    }

    // GET /auth/email-status
    [HttpGet("email-status")]
    public async Task<IActionResult> EmailStatus([FromQuery] string email)
    {
        var result = await _authService.GetEmailStatusAsync(email);
        return Ok(new ApiResponse<object>(true, "Email verification status retrieved successfully.", result, 200));
    }

    // GET /auth/resend-verify-email
    [HttpPost("resend-verify-email")]
    public async Task<IActionResult> Resend([FromBody] ResendVerifyEmailDto req)
    {
        var now = DateHelper.GetUtcNow();
        var message = "If an account exists, a verification email will be sent";
        const int status = StatusCodes.Status200OK;

        var user = await _authService.GetUserByEmailAsync(req.Email);
        if (user is null || user.EmailStatus == 'y')
        {
            return Ok(new ApiResponse<object>(true, message, null, status));
        }

        var lastCreated = await _emailVerificationService.GetLastUnconsumedCreatedAtAsync(user.Id, user.Email);
        if (lastCreated is not null && lastCreated.Value > now.AddMinutes(-2))
            return Ok(new ApiResponse<object>(true, message, null, status));

        // Optional daily cap (e.g., max 5 in 1 hour)
        var sentCount = await _emailVerificationService.CountCreatedSinceAsync(user.Id, user.Email, now.AddHours(-1));
        if (sentCount >= 5)
            return Ok(new ApiResponse<object>(true, message, null, status));

        await _emailVerificationService.SendLinkAsync(
            user.Id,
            user.Email,
            user.Nickname,
            now
        );

        return Ok(new ApiResponse<object>(true, message, null, status));
    }
}

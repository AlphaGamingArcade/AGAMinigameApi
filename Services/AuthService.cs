using AGAMinigameApi.Dtos.Auth;
using AGAMinigameApi.Dtos.Common;

namespace AGAMinigameApi.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<ForgotPasswordResponseDto>> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenDto request);
        Task<ApiResponse<ResetPasswordResponseDto>> ResetPasswordAsync(ResetPasswordDto request);
    }
}
using AGAMinigameApi.Dtos.Auth;

namespace AGAMinigameApi.Services
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenDto request);
        Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto request);
    }

    public class AuthService : IAuthService
    {

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            await Task.Delay(1000);
            return new LoginResponseDto();
        }
        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            await Task.Delay(1000);
            return new RegisterResponseDto();
        }
        public async Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordDto request)
        {
            await Task.Delay(1000);
            return new ForgotPasswordResponseDto();
        }
        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenDto request)
        {
            await Task.Delay(1000);
            return new RefreshTokenResponseDto();
        }
        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto request)
        {
            await Task.Delay(1000);
            return new ResetPasswordResponseDto();
        }
    }
}
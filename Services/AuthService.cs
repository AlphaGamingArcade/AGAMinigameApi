using AGAMinigameApi.Dtos.Auth;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Interfaces;
using AGAMinigameApi.Models;

namespace AGAMinigameApi.Services
{
    public interface IAuthService
    {
        Task<bool> UserExistsByEmailAsync(string email);
        Task<bool> UserExistsByAccountAsync(string email);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, Agent agent);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenDto request);
        Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto request);
    }

    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        
        public AuthService(IAuthRepository authRepository) {
            _authRepository = authRepository;   
        }

        public async Task<bool> UserExistsByEmailAsync(string email) => await _authRepository.UserExistsByEmailAsync(email);
        public async Task<bool> UserExistsByAccountAsync(string account) => await _authRepository.UserExistsByAccountAsync(account);
        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, Agent agent)
        {
            var dateTime = DateHelper.GetUtcNow();
            var user = new User
            {
                AgentId = agent.Id,
                Nickname = request.Nickname,
                Account = request.Account,
                Email = request.Email,
                Password = request.Password
            };
            var createdUser = await _authRepository.CreateUserAsync(user, dateTime);

            // GENERATE TOKENS

            var result = new RegisterResponseDto
            {

            };
            return result;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            await Task.Delay(1000);
            return new LoginResponseDto();
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
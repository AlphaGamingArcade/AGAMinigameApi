using AGAMinigameApi.Dtos.Auth;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;
using SlotsApi.Services;

namespace AGAMinigameApi.Services
{
    public interface IAuthService
    {
        Task<bool> UserExistsByEmailAsync(string email);
        Task<bool> UserExistsByAccountAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, Agent agent);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request, User user);
        Task LogoutAsync(int memberId);
        Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(int memberId);
        Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto request);
    }

    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly string _jwtIssuer;

        public AuthService(IConfiguration configuration, IAuthRepository authRepository, IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository)
        {
            _authRepository = authRepository;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtIssuer = configuration["JwtSettings:Issuer"] ?? throw new InvalidOperationException("JWT issues missing in config.");
        }

        private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(int memberId, DateTime nowUtc)
        {
            // 1) Generate tokens
            var accessToken  = _jwtTokenService.GenerateAccessToken(memberId.ToString());
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // 2) Clear existing refresh tokens for this issuer/member
            await _refreshTokenRepository.DeleteRefreshTokenByMemberIdAsync(memberId);

            // 3) Save new refresh token
            var refresh = new RefreshToken
            {
                MemberId  = memberId,
                Token     = refreshToken,
                Issuer    = _jwtIssuer,
                CreatedAt = nowUtc,
                ExpiresAt = nowUtc.AddDays(7),
                RevokedAt = null
            };

            await _refreshTokenRepository.CreateRefreshTokenAsync(refresh);

            return (accessToken, refreshToken);
        }

        public async Task<bool> UserExistsByEmailAsync(string email) => await _authRepository.UserExistsByEmailAsync(email);
        public async Task<bool> UserExistsByAccountAsync(string account) => await _authRepository.UserExistsByAccountAsync(account);
        public async Task<User?> GetUserByEmailAsync(string email) => await _authRepository.GetUserByEmailAsync(email);
        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, Agent agent)
        {
            var dateTime = DateHelper.GetUtcNow();

            var user = new User
            {
                AgentId  = agent.Id,
                Nickname = request.Nickname,
                Dob = request.Dob,
                Account  = request.Account,
                Email    = request.Email,
                Password = request.Password
            };

            var createdUser = await _authRepository.CreateUserAsync(user, dateTime);

            var (accessToken, refreshToken) = await GenerateTokensAsync(createdUser.Id, dateTime);

            return new RegisterResponseDto
            {
                AccessToken  = accessToken,
                RefreshToken = refreshToken
            };
        }
        
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, User user)
        {
            var dateTime = DateHelper.GetUtcNow();
            var (accessToken, refreshToken)  = await GenerateTokensAsync(user.Id, dateTime);

            return new LoginResponseDto
            {
                AccessToken  = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task LogoutAsync(int memberId) => await _refreshTokenRepository.DeleteRefreshTokenByMemberIdAsync(memberId);
        

        public async Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordDto request)
        {
            await Task.Delay(1000);
            return new ForgotPasswordResponseDto();
        }
        
        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(int memberId)
        {
            var dateTime = DateHelper.GetUtcNow();
            var (accessToken, refreshToken)  = await GenerateTokensAsync(memberId, dateTime);

            return new RefreshTokenResponseDto
            {
                AccessToken  = accessToken,
                RefreshToken = refreshToken
            };
        }
        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto request)
        {
            await Task.Delay(1000);
            return new ResetPasswordResponseDto();
        }
    }
}
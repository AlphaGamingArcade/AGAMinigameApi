using AGAMinigameApi.Dtos.Auth;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;
using Microsoft.Extensions.Options;
using SlotsApi.Services;
using SmptOptions;

namespace AGAMinigameApi.Services
{
    public interface IAuthService
    {
        Task<(bool EmailTaken, bool AccountTaken)> CheckUserConflictsAsync(string email, string account);
        Task<User?> GetUserByEmailAsync(string email);
        Task<EmailStatusResponseDto> GetEmailStatusAsync(string email);
        Task RegisterAsync(RegisterRequestDto request, Agent agent);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request, User user);
        Task LogoutAsync(int memberId);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(int memberId);
        Task ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task ResetPasswordAsync(int memberId, string tokenHash, string newPassword);
        Task SetEmailVerifiedAsync(string email, DateTime dateTime);
    }

    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IForgotPasswordService _forgotPasswordService;
        private readonly AppOptions _appOptions;

        public AuthService(IAuthRepository authRepository, IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository, IForgotPasswordService forgotPasswordService, IEmailVerificationService emailVerificationService, IOptions<AppOptions> appOptions)
        {
            _authRepository = authRepository;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _emailVerificationService = emailVerificationService;
            _forgotPasswordService = forgotPasswordService;
            _appOptions = appOptions.Value;
        }

        private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(int memberId, DateTime nowUtc)
        {
            // 1) Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(memberId.ToString());
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // 2) Clear existing refresh tokens for this issuer/member
            await _refreshTokenRepository.DeleteRefreshTokenByMemberIdAsync(memberId);

            // 3) Save new refresh token
            var refresh = new RefreshToken
            {
                MemberId = memberId,
                Token = refreshToken,
                AppKey = _appOptions.Key,
                CreatedAt = nowUtc,
                ExpiresAt = nowUtc.AddDays(7),
                RevokedAt = null
            };

            await _refreshTokenRepository.CreateRefreshTokenAsync(refresh);

            return (accessToken, refreshToken);
        }

        public async Task<(bool EmailTaken, bool AccountTaken)> CheckUserConflictsAsync(string email, string account) => await _authRepository.CheckUserConflictsAsync(email, account);
        public async Task<User?> GetUserByEmailAsync(string email) => await _authRepository.GetUserByEmailAsync(email);
        public async Task RegisterAsync(RegisterRequestDto request, Agent agent)
        {
            var dateTime = DateHelper.GetUtcNow();

            var user = new User
            {
                AgentId = agent.Id,
                Nickname = request.Nickname,
                Token = HashHelper.ComputeSHA256(request.Account),
                Dob = request.Dob,
                Account = request.Account,
                Email = request.Email,
                EmailStatus = 'n',
                Password = request.Password
            };

            var createdUser = await _authRepository.CreateUserAsync(user, dateTime);

            await _emailVerificationService.SendLinkAsync(
                createdUser.Id,
                createdUser.Email,
                createdUser.Nickname,
                dateTime
            );
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, User user)
        {
            var dateTime = DateHelper.GetUtcNow();
            var (accessToken, refreshToken) = await GenerateTokensAsync(user.Id, dateTime);

            return new LoginResponseDto
            {
                Sub = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task LogoutAsync(int memberId) => await _refreshTokenRepository.DeleteRefreshTokenByMemberIdAsync(memberId);


        public async Task ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var utcNow = DateHelper.GetUtcNow();
            
            var user = await GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                await Task.Delay(1000);
                return;
            }

            await _forgotPasswordService.SendLinkAsync(
                user.Id, 
                user.Email, 
                user.Nickname, 
                utcNow
            );
        }

        public async Task ResetPasswordAsync(int memberId, string tokenHash, string newPassword)
        {
            var utcNow = DateHelper.GetUtcNow();
            await _authRepository.UpdatePasswordAsync(memberId, newPassword);
            await _forgotPasswordService.MarkAsUsedAsync(tokenHash, utcNow);
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(int memberId)
        {
            var dateTime = DateHelper.GetUtcNow();
            var (accessToken, refreshToken) = await GenerateTokensAsync(memberId, dateTime);

            return new RefreshTokenResponseDto
            {
                Sub = memberId,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task SetEmailVerifiedAsync(string email, DateTime dateTime) => await _authRepository.SetEmailVerifiedAsync(email, dateTime);

        public async Task<EmailStatusResponseDto> GetEmailStatusAsync(string email)
        {
            var (isVerified, datetime) = await _authRepository.GetEmailStatusAsync(email);
            return new EmailStatusResponseDto
            {
                IsVerified = isVerified,
                Datetime = datetime
            };
        }
    }
}
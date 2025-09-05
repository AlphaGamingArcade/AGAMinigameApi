using AGAMinigameApi.Dtos.Auth;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;
using AGAMinigameApi.Services.EmailSender;
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
        Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordDto request);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(int memberId);
        Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto request);
        Task SetEmailVerifiedAsync(string email, DateTime dateTime);
    }

    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IEmailSender _emailSender;
        private readonly AppOptions _appOptions;

        public AuthService(IAuthRepository authRepository, IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository, IEmailVerificationService emailVerificationService, IEmailSender emailSender, IOptions<AppOptions> appOptions)
        {
            _authRepository = authRepository;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _emailVerificationService = emailVerificationService;
            _emailSender = emailSender;
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
                Issuer = _appOptions.Key,
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
                AccessToken = accessToken,
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
            var (accessToken, refreshToken) = await GenerateTokensAsync(memberId, dateTime);

            return new RefreshTokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordDto request)
        {
            await Task.Delay(1000);
            return new ResetPasswordResponseDto();
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
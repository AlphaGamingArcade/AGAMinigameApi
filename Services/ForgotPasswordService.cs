using System.Security.Cryptography;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;
using AGAMinigameApi.Services.EmailSender;
using Microsoft.Extensions.Options;
using SmptOptions;

namespace AGAMinigameApi.Services
{
    public interface IForgotPasswordService
    {
        Task<string> CreateResetTokenAsync(int memberId, string email, DateTime now);
        Task<bool> ValidateResetTokenAsync(string token);
        Task SendLinkAsync(int memberId, string email, string displayName, DateTime utcNow);
        Task<bool> ResetPasswordAsync(string token, string newPassword, string ipAddress);
    }

    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IForgotPasswordRepository _forgotPasswordRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IEmailSender _emailSender;
        private readonly IOptions<AppOptions> _appOptions;
        private const int TOKEN_EXPIRY_MINUTES = 5;

        public ForgotPasswordService(
            IForgotPasswordRepository forgotPasswordRepository,
            IMemberRepository memberRepository,
            IOptions<AppOptions> appOptions,
            IEmailSender emailSender
        )
        {
            _forgotPasswordRepository = forgotPasswordRepository;
            _memberRepository = memberRepository;
            _appOptions = appOptions;
            _emailSender = emailSender;
        }

        public async Task<string> CreateResetTokenAsync(int memberId, string email, DateTime now)
        {
            // Invalidate any existing tokens for this user
            await _forgotPasswordRepository.InvalidateUserTokensAsync(memberId, now);

            // 32 bytes random → Base64Url token sent to user
            var tokenBytes = new byte[32];
            RandomNumberGenerator.Fill(tokenBytes);
            var token = HashHelper.Base64UrlEncode(tokenBytes);
            var tokenHash = HashHelper.ComputeSHA256(token);
            var expiresAt = DateHelper.GetUtcNow().AddMinutes(TOKEN_EXPIRY_MINUTES);

            // Create forgot password record
            var forgotPassword = new ForgotPassword
            {
                MemberId = memberId,
                AppKey = _appOptions.Value.Key,
                Email = email,
                Token = tokenHash,
                ExpiresAt = expiresAt,
                IsUsed = 'n',
                CreatedAt = DateHelper.GetUtcNow()
            };

            await _forgotPasswordRepository.CreateAsync(forgotPassword);

            return token;
        }

        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            var now = DateHelper.GetUtcNow();
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var forgotPasswordRecord = await _forgotPasswordRepository.GetByTokenAsync(token, now);

            if (forgotPasswordRecord == null)
                return false;

            // Check if token is valid
            return forgotPasswordRecord.IsUsed == 'n' &&
                   forgotPasswordRecord.ExpiresAt > DateHelper.GetUtcNow();
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword, string ipAddress)
        {
            var now = DateHelper.GetUtcNow();

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
                return false;

            var forgotPasswordRecord = await _forgotPasswordRepository.GetByTokenAsync(token, now);

            if (forgotPasswordRecord == null)
                return false;

            // Validate token
            if (forgotPasswordRecord.IsUsed == 'y' ||
                forgotPasswordRecord.ExpiresAt <= DateHelper.GetUtcNow())
            {
                return false;
            }

            await _memberRepository.UpdatePasswordAsync(forgotPasswordRecord.MemberId, newPassword);

            // Mark token as used
            await _forgotPasswordRepository.MarkAsUsedAsync(forgotPasswordRecord.Id, DateHelper.GetUtcNow());

            return true;
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var cutoffDate = DateHelper.GetUtcNow().AddHours(-24); // Keep records for 24 hours for audit
            await _forgotPasswordRepository.DeleteExpiredTokensAsync(cutoffDate);
        }

        public async Task SendLinkAsync(int memberId, string email, string displayName, DateTime utcNow)
        {
            var token = await CreateResetTokenAsync(memberId, email, utcNow);
            var link = $"{_appOptions.Value.Url}/forgot-password/reset-password.html?token={token}";
            await _emailSender.SendForgotPasswordAsync(email, displayName, link);
        }
    }
}
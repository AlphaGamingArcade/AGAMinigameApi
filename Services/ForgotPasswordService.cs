using System.Security.Cryptography;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;

namespace AGAMinigameApi.Services
{
    public interface IForgotPasswordService
    {
        Task<string> CreateResetTokenAsync(string email);
        Task<bool> ValidateResetTokenAsync(string token);
        Task<bool> ResetPasswordAsync(string token, string newPassword, string ipAddress);
        Task CleanupExpiredTokensAsync();
    }

    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IForgotPasswordRepository _forgotPasswordRepository;
        private readonly IMemberRepository _memberRepository;
        private const int TOKEN_EXPIRY_MINUTES = 5;

        public ForgotPasswordService(
            IForgotPasswordRepository forgotPasswordRepository,
            IMemberRepository memberRepository)
        {
            _forgotPasswordRepository = forgotPasswordRepository;
            _memberRepository = memberRepository;
        }

        public async Task<string> CreateResetTokenAsync(string email)
        {
            var now = DateHelper.GetUtcNow();
            // Check if user exists
            var member = await _memberRepository.GetByEmailAsync(email);
            if (member == null)
            {
                return string.Empty;
            }

            // Invalidate any existing tokens for this user
            await _forgotPasswordRepository.InvalidateUserTokensAsync(member.Id, now);

            // Generate secure token
            var randomToken = GenerateSecureToken();
            var token = HashHelper.ComputeSHA256(randomToken);
            var expiresAt = DateHelper.GetUtcNow().AddMinutes(TOKEN_EXPIRY_MINUTES);

            // Create forgot password record
            var forgotPassword = new ForgotPassword
            {
                MemberId = member.Id,
                Email = email,
                Token = token,
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

        private static string GenerateSecureToken()
        {
            // Generate a cryptographically secure token
            using var rng = RandomNumberGenerator.Create();
            var tokenBytes = new byte[32];
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
using System.Security.Cryptography;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;
using AGAMinigameApi.Services.EmailSender;
using Microsoft.Extensions.Options;
using SmptOptions;

namespace AGAMinigameApi.Services
{
    public interface IEmailVerificationService
    {
        Task<string> CreateEmailVerificationAsync(long userId, string email, DateTime utcNow);
        Task<DateTime?> GetLastUnconsumedCreatedAtAsync(long userId, string email);
        Task<int> CountCreatedSinceAsync(long userId, string email, DateTime sinceUtc);
        Task SendLinkAsync(long userId, string email, string displayName, DateTime utcNow);
        Task<bool> VerifyAsync(string token, DateTime utcNow);
    }

    public sealed class EmailVerificationService : IEmailVerificationService
    {
        private readonly IEmailVerificationRepository _repo;
        private readonly IEmailSender _emailSender;
        private readonly IOptions<AppOptions> _appOptions;

        public EmailVerificationService(
            IEmailVerificationRepository repo,
            IEmailSender emailSender,
            IOptions<AppOptions> appOptions)
        {
            _repo = repo;
            _emailSender = emailSender;
            _appOptions = appOptions;
        }

        public async Task<string> CreateEmailVerificationAsync(long userId, string email, DateTime utcNow)
        {
            await _repo.InvalidateUnconsumedAsync(userId, email, utcNow);
            
            // 32 bytes random → Base64Url token sent to user
            var tokenBytes = new byte[32];
            RandomNumberGenerator.Fill(tokenBytes);
            var token = HashHelper.Base64UrlEncode(tokenBytes);
            var tokenHash = HashHelper.ComputeSHA256(token);

            // Store only the SHA-256 hash (bytes) in DB
            var entity = new EmailVerification
            {
                MemberId = checked((int)userId),
                Email = email,
                AppKey = _appOptions.Value.Key,
                TokenHash = tokenHash,
                Purpose = VerificationPurposes.EmailVerification,
                CreatedAtUtc = utcNow,
                ExpiresAtUtc = utcNow.AddMinutes(5),
                ConsumedAtUtc = null
            };

            await _repo.CreateEmailVerificationAsync(entity);
            
            return token; // return plaintext for composing the link
        }

        public async Task<bool> VerifyAsync(string token, DateTime utcNow)
        {
            var ev = await _repo.GetByTokenAsync(token);
            if (ev is null) return false;
            if (ev.ConsumedAtUtc is not null) return false;
            if (ev.ExpiresAtUtc <= utcNow) return false;
            return true;
        }

        public async Task<DateTime?> GetLastUnconsumedCreatedAtAsync(long userId, string email) => await _repo.GetLastUnconsumedCreatedAtAsync(userId, email);
        public async Task<int> CountCreatedSinceAsync(long userId, string email, DateTime sinceUtc) => await _repo.CountCreatedSinceAsync(userId, email, sinceUtc);

        public async Task SendLinkAsync(long userId, string email, string displayName, DateTime utcNow)
        {
            var token = await CreateEmailVerificationAsync(userId, email, utcNow);
            var link = $"{_appOptions.Value.Url}/auth/confirm-email?token={token}";
            await _emailSender.SendVerificationEmailAsync(email, displayName, link);
        }
    }
}
using System.Security.Cryptography;
using System.Text;
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
        Task InvalidateUnconsumedAsync(long userId, string email, DateTime utcNow);
        Task SendLinkAsync(long userId, string email, string displayName, DateTime utcNow);
        Task<bool> VerifyAsync(string token, DateTime utcNow);
    }

    public sealed class EmailVerificationService : IEmailVerificationService
    {
        private readonly IEmailVerificationRepository _repo;
        private readonly IEmailSender _emailSender;
        private readonly AppOptions _appOptions;

        public EmailVerificationService(
            IEmailVerificationRepository repo,
            IEmailSender emailSender,
            IOptions<AppOptions> appOptions)
        {
            _repo = repo;
            _emailSender = emailSender;
            _appOptions = appOptions.Value;
        }

        public async Task<string> CreateEmailVerificationAsync(long userId, string email, DateTime utcNow)
        {
            // 32 bytes random → Base64Url token sent to user
            var tokenBytes = new byte[32];
            RandomNumberGenerator.Fill(tokenBytes);
            var token = Base64UrlEncode(tokenBytes);
            var tokenHash = Sha256Hex(token);

            // Store only the SHA-256 hash (bytes) in DB
            var entity = new EmailVerification
            {
                MemberId = checked((int)userId),
                Email = email,
                AppKey = _appOptions.Key,
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

            await _repo.MarkUserEmailConfirmedAsync(ev.MemberId, ev.Email, utcNow);
            return true;
        }

        public async Task<DateTime?> GetLastUnconsumedCreatedAtAsync(long userId, string email) => await _repo.GetLastUnconsumedCreatedAtAsync(userId, email);
        public async Task<int> CountCreatedSinceAsync(long userId, string email, DateTime sinceUtc) => await _repo.CountCreatedSinceAsync(userId, email, sinceUtc);

        public async Task InvalidateUnconsumedAsync(long userId, string email, DateTime nowUtc)
            => await _repo.InvalidateUnconsumedAsync(userId, email, nowUtc);

        public async Task SendLinkAsync(long userId, string email, string displayName, DateTime utcNow)
        {
            var token = await CreateEmailVerificationAsync(userId, email, utcNow);
            var link = $"{_appOptions.Url}/auth/confirm-email?token={token}";
            await _emailSender.SendVerificationEmailAsync(email, displayName, link);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            var s = Convert.ToBase64String(bytes);
            return s.Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        private static string Sha256Hex(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes); // 64-char uppercase hex
        }
    }
}
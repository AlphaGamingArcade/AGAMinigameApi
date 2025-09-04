using System.Security.Cryptography;
using System.Text;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;
using Microsoft.Extensions.Options;
using SmptOptions;

namespace AGAMinigameApi.Services
{
    public interface IEmailSender
    {
        Task SendVerificationEmailAsync(string toEmail, string displayName, string verificationLink);
    }

    public interface IEmailVerificationService
    {
        Task<string> CreateEmailVerificationAsync(long userId, string email, DateTime utcNow);
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

            // Store only the SHA-256 hash (bytes) in DB
            var entity = new EmailVerification
            {
                MemberId = checked((int)userId),
                Email = email,
                AppKey = _appOptions.Key,
                Token = token,
                Purpose = "email_verification",
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

        // Helper to compose and send email in one call (optional)
        public async Task SendLinkAsync(long userId, string email, string displayName, DateTime utcNow)
        {
            var token = await CreateEmailVerificationAsync(userId, email, utcNow);
            var link = $"{_appOptions.Urls.PublicBaseUrl}/verify/email?token={token}";
            await _emailSender.SendVerificationEmailAsync(email, displayName, link);
        }

        private static byte[] Sha256Bytes(string input)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            var s = Convert.ToBase64String(bytes);
            return s.Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }
    }
}
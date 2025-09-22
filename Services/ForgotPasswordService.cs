using System.Security.Cryptography;
using AGAMinigameApi.Dtos.Auth;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;
using AGAMinigameApi.Services.EmailSender;
using api.Mappers;
using Microsoft.Extensions.Options;
using SmptOptions;

namespace AGAMinigameApi.Services
{
    public interface IForgotPasswordService
    {
        Task<string> CreateResetTokenAsync(int memberId, string email, DateTime now);
        Task SendLinkAsync(int memberId, string email, string displayName, DateTime utcNow);
        Task MarkAsUsedAsync(string tokenHash, DateTime now);
        Task<ForgotPasswordDto?> GetByTokenAsync(string tokenHash);
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

            Console.WriteLine($"WHEN REQUESTIONG {token} ========== {tokenHash}");

            // Create forgot password record
            var forgotPassword = new ForgotPassword
            {
                MemberId = memberId,
                AppKey = _appOptions.Value.Key,
                Email = email,
                TokenHash = tokenHash,
                ExpiresAt = expiresAt,
                CreatedAt = DateHelper.GetUtcNow()
            };

            await _forgotPasswordRepository.CreateAsync(forgotPassword);

            return token;
        }

        public async Task<ForgotPasswordDto?> GetByTokenAsync(string tokenHash)
        {
            var forgotPassword = await _forgotPasswordRepository.GetByTokenAsync(tokenHash);
            if (forgotPassword == null) return null;
            return forgotPassword.ToForgotPasswordDto(); 
        }
        
        public async Task MarkAsUsedAsync(string tokenHash, DateTime now) =>
            await _forgotPasswordRepository.MarkAsUsedAsync(tokenHash, now);
        public async Task SendLinkAsync(int memberId, string email, string displayName, DateTime utcNow)
        {
            var token = await CreateResetTokenAsync(memberId, email, utcNow);
            var link = $"{_appOptions.Value.Url}/forgot-password/reset-password.html?token={token}";
            await _emailSender.SendForgotPasswordAsync(email, displayName, link);
        }
    }
}
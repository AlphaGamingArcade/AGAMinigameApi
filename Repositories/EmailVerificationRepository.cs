using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IEmailVerificationRepository
    {
        Task<EmailVerification> CreateEmailVerificationAsync(EmailVerification emailVerification);
        Task<EmailVerification?> GetByTokenAsync(string tokenHash);
        Task MarkUserEmailConfirmedAsync(int memberId, string email, DateTime now);
    }

    public class EmailVerificationRepository : BaseRepository, IEmailVerificationRepository
    {
        public EmailVerificationRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<EmailVerification> CreateEmailVerificationAsync(EmailVerification ev)
        {
            const string query = @"
                INSERT INTO mg_email_verify
                (
                    email_verify_member_id,
                    email_verify_email,
                    email_verify_app_key,
                    email_verify_token_hash,
                    email_verify_purpose,
                    email_verify_created_at,
                    email_verify_expires_at,
                    email_verify_consumed_at
                )
                VALUES
                (
                    @memberId,
                    @email,
                    @appKey,
                    @tokenHash,
                    @purpose,
                    @createdAtUtc,
                    @expiresAtUtc,
                    @consumedAtUtc
                )";

            var parameters = new Dictionary<string, object>
            {
                ["@memberId"]     = ev.MemberId,
                ["@email"]        = ev.Email,
                ["@appKey"]       = (object?)ev.AppKey ?? DBNull.Value,
                ["@tokenHash"]    = ev.Token,
                ["@purpose"]      = string.IsNullOrWhiteSpace(ev.Purpose) ? "email_verify" : ev.Purpose,
                ["@createdAtUtc"] = ev.CreatedAtUtc,
                ["@expiresAtUtc"] = ev.ExpiresAtUtc,
                ["@consumedAtUtc"]= (object?)ev.ConsumedAtUtc ?? DBNull.Value
            };

            
            var newIdObj = await InsertQueryAsync(query, parameters);
            ev.Id = Convert.ToInt64(newIdObj);

            return ev;
        }

        public async Task<EmailVerification?> GetByTokenAsync(string tokenHash)
        {
            const string sql = @"
                SELECT TOP (1) *
                FROM mg_email_verify
                WHERE email_verify_token_hash = @tokenHash;";

            var parameters = new Dictionary<string, object>
            {
                ["@tokenHash"] = tokenHash
            };

            var table = await SelectQueryAsync(sql, parameters);
            if (table.Rows.Count == 0) return null;
            return table.Rows[0].ToEmailVerificationFromDataRow();
        }

        public async Task MarkUserEmailConfirmedAsync(int memberId, string email, DateTime now)
        {
            // 1) Mark verification rows consumed
            const string consumeSql = @"
                UPDATE mg_email_verify
                SET email_verify_consumed_at = @now
                WHERE email_verify_member_id = @memberId
                AND email_verify_email = @email
                AND email_verify_consumed_at IS NULL;";

            var parameters = new Dictionary<string, object>
            {
                ["@now"] = now,
                ["@memberId"] = memberId,
                ["@email"] = email
            };

            await UpdateQueryAsync(consumeSql, parameters);
        }
    }
}

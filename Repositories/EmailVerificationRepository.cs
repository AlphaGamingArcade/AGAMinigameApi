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
        Task<DateTime?> GetLastUnconsumedCreatedAtAsync(long userId, string email);
        Task<int> CountCreatedSinceAsync(long userId, string email, DateTime sinceUtc);
        Task InvalidateUnconsumedAsync(long userId, string email, DateTime nowUtc);
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
                ["@memberId"] = ev.MemberId,
                ["@email"] = ev.Email,
                ["@appKey"] = (object?)ev.AppKey ?? DBNull.Value,
                ["@tokenHash"] = ev.TokenHash,
                ["@purpose"] = string.IsNullOrWhiteSpace(ev.Purpose) ? "email_verify" : ev.Purpose,
                ["@createdAtUtc"] = ev.CreatedAtUtc,
                ["@expiresAtUtc"] = ev.ExpiresAtUtc,
                ["@consumedAtUtc"] = (object?)ev.ConsumedAtUtc ?? DBNull.Value
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

        public async Task InvalidateUnconsumedAsync(long userId, string email, DateTime nowUtc)
        {
            const string sql = @"
                UPDATE mg_email_verify
                SET email_verify_consumed_at = @nowUtc
                WHERE email_verify_member_id = @memberId
                AND email_verify_email = @email
                AND email_verify_consumed_at IS NULL
                AND email_verify_purpose = @purpose;";

            var p = new Dictionary<string, object>
            {
                ["@nowUtc"] = nowUtc,
                ["@memberId"] = userId,
                ["@email"] = email,
                ["@purpose"] = "email_verification"
            };
            await SelectQueryAsync(sql, p); // using your existing helper
        }

        public async Task<DateTime?> GetLastUnconsumedCreatedAtAsync(long userId, string email)
        {
            const string sql = @"
                SELECT MAX(email_verify_created_at) AS last_created_at
                FROM mg_email_verify
                WHERE email_verify_member_id = @memberId
                AND email_verify_email = @email
                AND email_verify_consumed_at IS NULL
                AND email_verify_purpose = @purpose;";

                var p = new Dictionary<string, object>
                {
                    ["@memberId"] = userId,
                    ["@email"] = email,
                    ["@purpose"] = "email_verification"
                };
                var dt = await SelectQueryAsync(sql, p);
                if (dt.Rows.Count == 0 || dt.Rows[0].IsNull("last_created_at"))
                    return null;
                return Convert.ToDateTime(dt.Rows[0]["last_created_at"]);
        }

        public async Task<int> CountCreatedSinceAsync(long userId, string email, DateTime sinceUtc)
        {
            const string sql = @"
                SELECT COUNT(1) AS cnt
                FROM mg_email_verify
                WHERE email_verify_member_id = @memberId
                AND email_verify_email = @email
                AND email_verify_created_at >= @sinceUtc
                AND email_verify_purpose = @purpose;";

            var p = new Dictionary<string, object>
            {
                ["@memberId"] = userId,
                ["@email"] = email,
                ["@sinceUtc"] = sinceUtc,
                ["@purpose"] = "email_verification"
            };
            var dt = await SelectQueryAsync(sql, p);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["cnt"]) : 0;
        }

    }
}

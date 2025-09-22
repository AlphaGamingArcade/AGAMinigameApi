
using AGAMinigameApi.Models;
using api.Mappers;
using Microsoft.Extensions.Options;
using SmptOptions;

namespace AGAMinigameApi.Repositories
{
    public interface IForgotPasswordRepository
    {
        Task CreateAsync(ForgotPassword forgotPassword);
        Task<ForgotPassword?> GetByTokenAsync(string token, DateTime now);
        Task InvalidateUserTokensAsync(long memberId, DateTime now);
        Task MarkAsUsedAsync(int forgotPasswordId, DateTime usedAt);
        Task DeleteExpiredTokensAsync(DateTime cutoffDate);
    }

    public class ForgotPasswordRepository : BaseRepository, IForgotPasswordRepository
    {
        private readonly IOptions<AppOptions> _appOptions;
                
        public ForgotPasswordRepository(IConfiguration configuration, IOptions<AppOptions> appOptions) : base(configuration)
        {
            _appOptions = appOptions;
        }

        public async Task CreateAsync(ForgotPassword forgotPassword)
        {
            const string query = @"
                INSERT INTO mg_forgot_password 
                (
                    forgot_password_member_id,
                    forgot_password_email,
                    forgot_password_app_key,
                    forgot_password_token,
                    forgot_password_created_at,
                    forgot_password_expires_at,
                    forgot_password_is_used,
                    forgot_password_used_at
                )
                VALUES 
                (
                    @memberId,
                    @email,
                    @appKey,
                    @token,
                    @createdAt,
                    @expiresAt,
                    @isUsed,
                    @usedAt
                );";

            var parameters = new Dictionary<string, object>
            {
                { "@memberId", forgotPassword.MemberId },
                { "@email", forgotPassword.Email },
                { "@appKey", forgotPassword.AppKey },
                { "@token", forgotPassword.Token },
                { "@createdAt", forgotPassword.CreatedAt },
                { "@expiresAt", forgotPassword.ExpiresAt },
                { "@isUsed", forgotPassword.IsUsed },
                { "@usedAt", (object?)forgotPassword.UsedAt ?? DBNull.Value }
            };

            await InsertQueryAsync(query, parameters);
        }

        public async Task<ForgotPassword?> GetByTokenAsync(string token, DateTime now)
        {
            const string query = @"
                SELECT 
                    forgot_password_id,
                    forgot_password_member_id,
                    forgot_password_email,
                    forgot_password_app_key,
                    forgot_password_token,
                    forgot_password_created_at,
                    forgot_password_expires_at,
                    forgot_password_is_used,
                    forgot_password_used_at
                FROM mg_forgot_password
                WHERE forgot_password_app_key = @appKey 
                AND forgot_password_token = @token
                AND forgot_password_is_used = 'n'
                AND forgot_password_expires_at > @now
                LIMIT 1;";

            var parameters = new Dictionary<string, object>
            {
                { "@appKey", _appOptions.Value.Key},
                { "@token", token },
                { "@now", now }
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return row.ToForgotPasswordFromDataRow();
            }
            return null;
        }

        public async Task InvalidateUserTokensAsync(long memberId, DateTime now)
        {
            const string query = @"
                DELETE mg_forgot_password
                WHERE forgot_password_app_key = @appKey 
                AND forgot_password_member_id = @memberId;";

            var parameters = new Dictionary<string, object>
            {
                { "@appKey", _appOptions.Value.Key },
                { "@memberId", memberId },
                { "@now", now}
            };

            await UpdateQueryAsync(query, parameters);
        }

        public async Task MarkAsUsedAsync(int forgotPasswordId, DateTime usedAt)
        {
            const string query = @"
                UPDATE mg_forgot_password
                SET 
                    forgot_password_is_used = 'y',
                    forgot_password_used_at = @usedAt
                WHERE forgot_password_app_key = @appKey AND forgot_password_id = @forgotPasswordId;";

            var parameters = new Dictionary<string, object>
            {
                {  "@appKey", _appOptions.Value.Key },
                { "@forgotPasswordId", forgotPasswordId },
                { "@usedAt", usedAt }
            };

            await UpdateQueryAsync(query, parameters);
        }

        public async Task DeleteExpiredTokensAsync(DateTime cutoffDate)
        {
            const string query = @"
                DELETE FROM mg_forgot_password
                WHERE forgot_password_app_key = @appKey 
                AND (forgot_password_created_at < @cutoffDate OR (forgot_password_is_used = 'y' AND forgot_password_used_at < @cutoffDate));";

            var parameters = new Dictionary<string, object>
            {
                { "@appKey", _appOptions.Value.Key },
                { "@cutoffDate", cutoffDate }
            };

            await DeleteQueryAsync(query, parameters);
        }
    }
}
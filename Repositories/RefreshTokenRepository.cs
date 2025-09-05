using AGAMinigameApi.Models;
using api.Mappers;
using Microsoft.Extensions.Options;
using SmptOptions;

namespace AGAMinigameApi.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
        Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken);
        Task<int> DeleteRefreshTokenByMemberIdAsync(int memberId);
    }

    public class RefreshTokenRepository : BaseRepository, IRefreshTokenRepository
    {
        private readonly ILogger<RefreshTokenRepository> _logger;
        private readonly IOptions<AppOptions> _appOptions;

        public RefreshTokenRepository(IConfiguration configuration, IOptions<AppOptions> appOptions, ILogger<RefreshTokenRepository> logger)
            : base(configuration)
        {
            _appOptions = appOptions;
            _logger = logger;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
        {
            try
            {
                string sql = @"
                    SELECT 
                        t.refresh_token_id,
                        t.refresh_token_token,
                        t.refresh_token_app_key,
                        t.refresh_token_member_id,
                        t.refresh_token_created_at,
                        t.refresh_token_expires_at,
                        m.member_id
                    FROM mg_refresh_token t
                    INNER JOIN mg_member m ON t.refresh_token_member_id = m.member_id
                    WHERE t.refresh_token_token = @refreshToken AND t.refresh_token_revoked_at IS NULL AND t.refresh_token_app_key = @appKey";

                var parameters = new Dictionary<string, object>
                {
                    { "@refreshToken", refreshToken },
                    { "@appKey", _appOptions.Value.Key }
                };

                var result = await SelectQueryAsync(sql, parameters);
                if (result.Rows.Count == 0)
                    return null;

                return result.Rows[0].ToRefreshTokenFromDataRow();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting refresh token: {RefreshToken}", refreshToken);
                throw new ApplicationException("An error occurred while retrieving the refresh token.", ex);
            }
        }

        public async Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken)
        {
            try
            {
                string sql = @"
                    INSERT INTO mg_refresh_token (
                        refresh_token_member_id,
                        refresh_token_token,
                        refresh_token_app_key,
                        refresh_token_expires_at,
                        refresh_token_created_at,
                        refresh_token_revoked_at
                    )
                    VALUES (
                        @memberId,
                        @token,
                        @appKey,
                        @expiresAt,
                        @createdAt,
                        @revokedAt
                    )";

                var parameters = new Dictionary<string, object>
                {
                    { "@memberId", refreshToken.MemberId },
                    { "@token", refreshToken.Token },
                    { "@appKey", refreshToken.AppKey },
                    { "@expiresAt", refreshToken.ExpiresAt },
                    { "@createdAt", refreshToken.CreatedAt },
                    { "@revokedAt", (object?)refreshToken.RevokedAt ?? DBNull.Value }
                };


                var newIdObj = await InsertQueryAsync(sql, parameters);
                refreshToken.Id = Convert.ToInt32(newIdObj);

                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating refresh token for MemberId: {MemberId}", refreshToken.MemberId);
                throw new ApplicationException("An error occurred while creating the refresh token.", ex);
            }
        }

        public async Task<int> DeleteRefreshTokenByMemberIdAsync(int memberId)
        {
            try
            {
                string sql = "DELETE FROM mg_refresh_token WHERE refresh_token_member_id = @memberId AND refresh_token_app_key = @appKey";

                var parameters = new Dictionary<string, object>
                {
                    { "@memberId", memberId },
                    { "@appKey", _appOptions.Value.Key }
                };

                return await DeleteQueryAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting refresh token for MemberId: {MemberId}", memberId);
                throw new ApplicationException("An error occurred while deleting the refresh token.", ex);
            }
        }
    }
}

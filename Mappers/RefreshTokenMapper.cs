using System.Data;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class RefreshTokenMapper
    {
        public static RefreshToken ToRefreshTokenFromDataRow(this DataRow row)
        {
            return new RefreshToken
            {
                Id = Convert.ToInt32(row["refresh_token_id"]),
                Token = Convert.ToString(row["refresh_token_token"]) ?? string.Empty,
                MemberId = Convert.ToInt32(row["refresh_token_member_id"]),
                CreatedAt = Convert.ToDateTime(row["refresh_token_created_at"]),
                ExpiresAt = Convert.ToDateTime(row["refresh_token_expires_at"]),
            };
        }
    }
}
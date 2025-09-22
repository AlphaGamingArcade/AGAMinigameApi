using System.Data;
using AGAMinigameApi.Dtos.Auth;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static partial class DataRowExtensions
    {
        public static ForgotPassword ToForgotPasswordFromDataRow(this DataRow row)
        {
            return new ForgotPassword
            {
                Id = Convert.ToInt32(row["forgot_password_id"]),
                MemberId = Convert.ToInt32(row["forgot_password_member_id"]),
                Email = row["forgot_password_email"].ToString() ?? string.Empty,
                TokenHash = row["forgot_password_token_hash"].ToString() ?? string.Empty,
                CreatedAt = Convert.ToDateTime(row["forgot_password_created_at"]),
                ExpiresAt = Convert.ToDateTime(row["forgot_password_expires_at"]),
                ConsumedAt = row["forgot_password_consumed_at"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(row["forgot_password_consumed_at"])
            };
        }

        public static ForgotPasswordDto ToForgotPasswordDto(this ForgotPassword forgotPasswordModel)
        {
            return new ForgotPasswordDto
            {
                MemberId = forgotPasswordModel.MemberId,
                Token = forgotPasswordModel.TokenHash,
                CreatedAt = forgotPasswordModel.CreatedAt,
                ExpiresAt = forgotPasswordModel.ExpiresAt,
                ConsumedAt = forgotPasswordModel.ConsumedAt,
            };
        }
        
    }
}
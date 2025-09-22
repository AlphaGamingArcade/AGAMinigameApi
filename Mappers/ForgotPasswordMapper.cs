using System.Data;
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
                Token = row["forgot_password_token"].ToString() ?? string.Empty,
                CreatedAt = Convert.ToDateTime(row["forgot_password_created_at"]),
                ExpiresAt = Convert.ToDateTime(row["forgot_password_expires_at"]),
                IsUsed = Convert.ToChar(row["forgot_password_is_used"]),
                UsedAt = row["forgot_password_used_at"] == DBNull.Value 
                    ? null 
                    : Convert.ToDateTime(row["forgot_password_used_at"])
            };
        }
    }
}
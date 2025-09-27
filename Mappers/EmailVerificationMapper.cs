using System.Data;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class EmailVerificationMapper
    {
        public static EmailVerification ToEmailVerificationFromDataRow(this DataRow reader)
        {
            return new EmailVerification
            {
                Id            = Convert.ToInt64(reader["email_verify_id"]),          // bigint → long
                MemberId      = Convert.ToInt32(reader["email_verify_member_id"]),
                Email         = Convert.ToString(reader["email_verify_email"]) ?? "",
                AppKey        = Convert.ToString(reader["email_verify_app_key"]),
                TokenHash     = Convert.ToString(reader["email_verify_token_hash"]) ?? "", // <- fixed
                Purpose       = Convert.ToString(reader["email_verify_purpose"]) ?? "email_verify",
                CreatedAtUtc  = reader.IsNull("email_verify_created_at") ? DateTime.MinValue : Convert.ToDateTime(reader["email_verify_created_at"]),
                ExpiresAtUtc  = reader.IsNull("email_verify_expires_at") ? DateTime.MinValue : Convert.ToDateTime(reader["email_verify_expires_at"]),
                ConsumedAtUtc = reader.IsNull("email_verify_consumed_at") ? (DateTime?)null : Convert.ToDateTime(reader["email_verify_consumed_at"]),
            };
        }
    }
}
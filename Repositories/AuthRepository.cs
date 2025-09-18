using AGAMinigameApi.Helpers;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public record EmailStatusDto(bool IsVerified, DateTime? VerifiedAt);
    public interface IAuthRepository
    {
        Task SetEmailVerifiedAsync(string email, DateTime datetime);
        Task<(bool EmailTaken, bool AccountTaken)> CheckUserConflictsAsync(string email, string account);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user, DateTime dateTime);
        Task UpdatePasswordAsync(int userId, string newPassword);
        Task<(bool isVerified, DateTime? datetime)> GetEmailStatusAsync(string email);
    }

    public class AuthRepository : BaseRepository, IAuthRepository
    {
        public AuthRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<(bool EmailTaken, bool AccountTaken)> CheckUserConflictsAsync(string email, string account)
        {
            const string sql = @"
                SELECT 
                    EmailTaken   = CASE WHEN EXISTS (SELECT 1 FROM mg_app_user where app_user_email = @email) THEN 1 ELSE 0 END,
                    AccountTaken = CASE WHEN EXISTS (SELECT 1 FROM mg_member WHERE member_account = @account) THEN 1 ELSE 0 END;";

            var p = new Dictionary<string, object>
            {
                ["@email"] = email,
                ["@account"] = account
            };

            var table = await SelectQueryAsync(sql, p);
            if (table.Rows.Count == 0) return (false, false);

            var row = table.Rows[0];
            var emailTaken = Convert.ToInt32(row["EmailTaken"]) == 1;
            var accountTaken = Convert.ToInt32(row["AccountTaken"]) == 1;

            return (emailTaken, accountTaken);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            const string query = @"SELECT 
                    au.app_user_member_id,
                    au.app_user_email,
                    au.app_user_email_status,
                    au.app_user_password
                FROM mg_app_user au
                INNER JOIN mg_member m ON m.member_id = au.app_user_member_id
                WHERE au.app_user_email = @email;";
            var parameters = new Dictionary<string, object>
            {
                { "@email", email }
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return row.ToUserFromDataRow();
            }
            return null;
        }

        public async Task<User> CreateUserAsync(User user, DateTime dateTime)
        {
            const string sql = @"
                SET XACT_ABORT ON;
                BEGIN TRAN;

                DECLARE @now DATETIME2(3) = @datetime;
                DECLARE @new_member TABLE (member_id INT);

                INSERT INTO dbo.mg_member (
                    member_agent_id,
                    member_account,
                    member_nickname,
                    member_gamemoney,
                    member_charge_money,
                    member_exchange_money,
                    member_betting_money,
                    member_betting_benefit_money,
                    member_token,
                    member_online,
                    member_status,
                    member_datetime,
                    member_update,
                    member_level
                )
                OUTPUT inserted.member_id INTO @new_member(member_id)
                VALUES (
                    @agent_id,
                    @account,
                    @nickname,
                    0, 0, 0, 0, 0,
                    @token,
                    'n', 'y',
                    @now,
                    @now,
                    @level
                );

                INSERT INTO dbo.mg_app_user (
                    app_user_member_id,
                    app_user_email,
                    app_user_password,
                    app_user_email_status,
                    app_user_dob,
                    app_user_created_at,
                    app_user_updated_at
                )
                SELECT
                    m.member_id,
                    @email,
                    @password,     
                    @emailStatus,
                    @dob,
                    @now,
                    @now
                FROM @new_member m;

                SELECT member_id FROM @new_member;

                COMMIT TRAN;
            ";

            var parameters = new Dictionary<string, object>
            {
                ["@agent_id"] = user.AgentId,
                ["@account"] = user.Account,
                ["@nickname"] = user.Nickname,
                ["@token"] = user.Token,
                ["@level"] = 1,
                ["@datetime"] = dateTime,

                ["@email"] = user.Email,
                ["@password"] = user.Password,        // HASHED
                ["@emailStatus"] = user.EmailStatus,     // e.g. 'n','p','v','b'
                ["@dob"] = (object?)user.Dob ?? DBNull.Value,
            };

            var newIdObj = await InsertQueryAsync(sql, parameters);
            user.Id = Convert.ToInt32(newIdObj);
            return user;
        }

        public async Task UpdatePasswordAsync(int userId, string newPassword)
        {
            const string query = @"
                UPDATE mg_app_user
                SET app_user_password = @newPassword
                WHERE app_user_member_id = @userId;";

            var parameters = new Dictionary<string, object>
            {
                ["@newPassword"] = newPassword,
                ["@userId"] = userId
            };

            await UpdateQueryAsync(query, parameters);
        }

        public async Task SetEmailVerifiedAsync(string tokenHash, DateTime dateTime)
        {
            const string query = @"
                -- 1) Mark app_user as verified
                UPDATE au
                SET au.app_user_email_status = 'y',
                    au.app_user_updated_at   = @datetime
                FROM dbo.mg_app_user au
                INNER JOIN dbo.mg_email_verify ev 
                    ON au.app_user_email = ev.email_verify_email
                WHERE ev.email_verify_token_hash = @tokenHash
                AND ev.email_verify_consumed_at IS NULL;

                -- 2) Mark the token as consumed
                UPDATE dbo.mg_email_verify
                SET email_verify_consumed_at = @datetime
                WHERE email_verify_token_hash = @tokenHash
                AND email_verify_consumed_at IS NULL;
            ";
            var parameters = new Dictionary<string, object>
            {
                ["@tokenHash"] = tokenHash,
                ["@datetime"] = dateTime
            };

            await UpdateQueryAsync(query, parameters);
        }

        public async Task<(bool isVerified, DateTime? datetime)> GetEmailStatusAsync(string email)
        {
            const string query = @"
                SELECT TOP 1 ev.email_verify_consumed_at
                FROM dbo.mg_app_user au
                INNER JOIN dbo.mg_email_verify ev 
                    ON ev.email_verify_email = au.app_user_email
                WHERE au.app_user_email_status = 'y'
                AND au.app_user_email = @email
                AND ev.email_verify_purpose = @purpose
                AND ev.email_verify_consumed_at IS NOT NULL
                ORDER BY ev.email_verify_consumed_at DESC;
            ";
            
            var parameters = new Dictionary<string, object>
            {
                ["@email"] = email,
                ["@purpose"] = "email_verification"
            };

            var dt = await SelectQueryAsync(query, parameters);

            if (dt.Rows.Count > 0)
            {
                var consumedAt = dt.Rows[0]["email_verify_consumed_at"] as DateTime?;
                return (true, consumedAt);
            }

            return (false, null);
        }
    }
}
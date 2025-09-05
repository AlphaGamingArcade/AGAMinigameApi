using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IAuthRepository
    {
        Task SetEmailVerifiedAsync(string email, DateTime datetime);
        Task<(bool EmailTaken, bool AccountTaken)> CheckUserConflictsAsync(string email, string account);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user, DateTime dateTime);
        Task UpdatePasswordAsync(int userId, string newPassword);
    }

    public class AuthRepository : BaseRepository, IAuthRepository
    {
        public AuthRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<(bool EmailTaken, bool AccountTaken)> CheckUserConflictsAsync(string email, string account)
        {
            const string sql = @"
                SELECT 
                    EmailTaken   = CASE WHEN EXISTS (SELECT 1 FROM mg_member WHERE member_email   = @Email)   THEN 1 ELSE 0 END,
                    AccountTaken = CASE WHEN EXISTS (SELECT 1 FROM mg_member WHERE member_account = @Account) THEN 1 ELSE 0 END;";

            var p = new Dictionary<string, object>
            {
                ["@Email"] = email,
                ["@Account"] = account
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
            const string query = "SELECT member_id, member_email, member_email_status, member_password FROM mg_member WHERE member_email = @email;";
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
            const string query = @"
                INSERT INTO dbo.mg_member (
                    member_agent_id,
                    member_account,
                    member_nickname,
                    member_email,
                    member_email_status,
                    member_password,
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
                    member_level,
                    member_dob
                )
                OUTPUT inserted.member_id
                VALUES (
                    @agent_id,
                    @account,
                    @nickname,
                    @email,
                    @emailStatus,
                    @password,
                    0, 0, 0, 0, 0,
                    @token,
                    'n', 'y',
                    @createdAt,
                    @updatedAt,
                    @level,
                    @dob
                );";

            var parameters = new Dictionary<string, object>
            {
                ["@agent_id"] = user.AgentId,
                ["@account"] = user.Account,
                ["@nickname"] = user.Nickname,
                ["@email"] = user.Email,
                ["@emailStatus"] = user.EmailStatus,
                ["@password"] = user.Password,
                ["@token"] = string.Empty,
                ["@createdAt"] = dateTime,
                ["@updatedAt"] = dateTime,
                ["@level"] = 1,
                ["@dob"] = user.Dob
            };

            var newIdObj = await InsertQueryAsync(query, parameters);
            user.Id = Convert.ToInt32(newIdObj);

            return user;
        }

        public async Task UpdatePasswordAsync(int userId, string newPassword)
        {
            const string query = @"
                UPDATE mg_member
                SET member_password = @newPassword
                WHERE member_id = @userId;";

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
                UPDATE m
                SET m.member_email_status = 'y'
                FROM mg_member m
                JOIN mg_email_verify ev 
                    ON m.member_email = ev.email_verify_email
                WHERE ev.email_verify_token_hash = @tokenHash
                AND ev.email_verify_consumed_at IS NULL;

                UPDATE mg_email_verify
                SET email_verify_consumed_at = @dateTime
                WHERE email_verify_token_hash = @tokenHash
                AND email_verify_consumed_at IS NULL;
            ";

            var parameters = new Dictionary<string, object>
            {
                ["@tokenHash"] = tokenHash,
                ["@dateTime"] = dateTime
            };

            await UpdateQueryAsync(query, parameters);
        }
    }
}
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> UserExistsByEmailAsync(string email);
        Task<bool> UserExistsByAccountAsync(string account);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user, DateTime dateTime);
        Task UpdatePasswordAsync(int userId, string newPassword);
    }

    public class AuthRepository : BaseRepository, IAuthRepository
    {
        public AuthRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            const string query = "SELECT member_email FROM mg_member WHERE member_email = @email;";
            var parameters = new Dictionary<string, object>
            {
                { "@email", email }
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            return dataTable.Rows.Count > 0;
        }

        public async Task<bool> UserExistsByAccountAsync(string account)
        {
            const string query = "SELECT member_account FROM mg_member WHERE member_account = @account;";
            var parameters = new Dictionary<string, object>
            {
                { "@account", account }
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            return dataTable.Rows.Count > 0;
        }


        public async Task<User?> GetUserByEmailAsync(string email)
        {
            const string query = "SELECT member_id, member_email, member_password FROM mg_member WHERE member_email = @email;";
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
                    member_free_spin_status,
                    member_jackpot_status,
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
                    'n', 'n', 'n', 'y',
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
    }
}
using AGAMinigameApi.Interfaces;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public class AuthRepository : BaseRepository, IAuthRepository
    {
        public AuthRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<bool> UserExistsAsync(string email)
        {
            const string query = "SELECT email FROM mg_member WHERE member_email = @email;";
            var parameters = new Dictionary<string, object>
            {
                { "@email", email }
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

        public async Task<User> CreateUserAsync(User user)
        {
            const string query = @"
                INSERT INTO mg_member (member_username, member_email, member_password)
                VALUES (@username, @email, @password);";

            var parameters = new Dictionary<string, object>
            {
                ["@username"] = user.Email,
                ["@email"] = user.Email,
                ["@password"] = user.Password
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
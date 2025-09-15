using System.Data;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class UserMapper
    {
        public static User ToUserFromDataRow(this DataRow reader)
        {
            return new User
            {
                Id = Convert.ToInt32(reader["app_user_member_id"]),
                Email = Convert.ToString(reader["app_user_email"])!,
                EmailStatus = Convert.ToChar(reader["app_user_email_status"])!,
                Password = Convert.ToString(reader["app_user_password"])!,
            };
        }
    }
}
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
                Id = Convert.ToInt32(reader["member_id"]),
                Email = Convert.ToString(reader["member_email"])!,
                Password = Convert.ToString(reader["member_password"])!,
            };
        }
    }
}
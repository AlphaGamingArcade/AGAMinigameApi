using System.Data;
using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class MemberMapper
    {
        public static Member ToMemberFromDataRow(this DataRow reader)
        {
            return new Member
            {
                Id = Convert.ToInt32(reader["app_user_member_id"]),
                Email = Convert.ToString(reader["app_user_email"]) ?? "",
                Account = Convert.ToString(reader["member_account"]) ?? "",
                Nickname = Convert.ToString(reader["member_nickname"]) ?? "",
                Gamemoney = Convert.ToDecimal(reader["member_gamemoney"]),
            };
        }
        
        public static MemberDto ToMemberDto(this Member memberModel)
        {
            return new MemberDto
            {
                Id = memberModel.Id,
                Email = memberModel.Email,
                Account = memberModel.Account,
                Nickname = memberModel.Nickname,
                Gamemoney =  memberModel.Gamemoney,
            };
        }
    }
}
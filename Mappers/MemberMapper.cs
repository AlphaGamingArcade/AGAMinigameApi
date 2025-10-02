using System.Data;
using AGAMinigameApi.Dtos.Member;
using AGAMinigameApi.Models;

namespace api.Mappers
{
    public static class MemberMapper
    {
        public static Member ToMemberFromDataRow(this DataRow reader)
        {
            return new Member
            {
                Id = Convert.ToInt32(reader["user_member_id"]),
                AgentId = Convert.ToInt16(reader["member_agent_id"]),
                Email = Convert.ToString(reader["user_email"]) ?? "",
                Account = Convert.ToString(reader["member_account"]) ?? "",
                Nickname = Convert.ToString(reader["member_nickname"]) ?? "",
                Gamemoney = Convert.ToDecimal(reader["member_gamemoney"]),
                Currency = Convert.ToString(reader["agent_currency"]) ?? "",
                Token = Convert.ToString(reader["member_token"]) ?? "",
                NicknameUpdate = reader["member_nickname_update"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["member_nickname_update"])
            };
        }
        
        public static MemberDto ToMemberDto(this Member memberModel)
        {
            return new MemberDto
            {
                Id = memberModel.Id,
                AgentId = memberModel.AgentId,
                Email = memberModel.Email,
                Account = memberModel.Account,
                Nickname = memberModel.Nickname,
                Gamemoney = memberModel.Gamemoney,
                Currency = memberModel.Currency,
                Token = memberModel.Token,
                NicknameUpdate = memberModel.NicknameUpdate
            };
        }
    }
}
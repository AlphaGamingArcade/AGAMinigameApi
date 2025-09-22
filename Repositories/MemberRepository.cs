using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IMemberRepository
    {
        Task<Member?> GetByIdAsync(int id);
        Task UpdateOnChargeAsync(int memberId, decimal amount);
        Task PatchNicknameAsync(int memberId, string nickname, DateTime datetime);
    }

    public class MemberRepository : BaseRepository, IMemberRepository
    {
        public MemberRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<Member?> GetByIdAsync(int id)
        {
            const string query = @"SELECT 
                    au.app_user_member_id,
                    au.app_user_email,
                    m.member_agent_id,
                    m.member_account,
                    m.member_nickname,
                    m.member_gamemoney,
                    m.member_token,
                    m.member_nickname_update,
                    a.agent_currency
                FROM mg_app_user au
                INNER JOIN mg_member m ON m.member_id = au.app_user_member_id
                INNER JOIN mg_agent a ON a.agent_id = m.member_agent_id
                WHERE au.app_user_member_id = @id;";
            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return row.ToMemberFromDataRow();
            }
            return null;
        }

        public async Task UpdateOnChargeAsync(int memberId, decimal amount)
        {
            const string query = @"
                UPDATE mg_member
                SET 
                    member_gamemoney = member_gamemoney + @amount,
                    member_charge_money = member_charge_money + @amount
                WHERE member_id = @memberId;";

            var parameters = new Dictionary<string, object>
            {
                { "@amount", amount },
                { "@memberId", memberId }
            };

            await UpdateQueryAsync(query, parameters);
        }

        public async Task PatchNicknameAsync(int memberId, string nickname, DateTime datetime)
        {
            const string query = @"
                UPDATE mg_member
                SET 
                    member_nickname = @nickname,
                    member_nickname_update = @datetime
                WHERE member_id = @memberId;";

            var parameters = new Dictionary<string, object>
            {
                { "@nickname", nickname },
                { "@memberId", memberId },
                { "@datetime", datetime}
            };

            await UpdateQueryAsync(query, parameters);
        }
    }
}
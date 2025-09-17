using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IMemberRepository
    {
        // Task<(List<Game> items, int total)> GetPaginatedGamesAsync(
        //     string? sortBy,
        //     bool descending,
        //     int pageNumber,
        //     int pageSize
        // );
        Task<Member?> GetByIdAsync(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }

    public class MemberRepository : BaseRepository, IMemberRepository
    {
        public MemberRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<Member?> GetByIdAsync(int id)
        {
            const string query = @"SELECT 
                    au.app_user_member_id,
                    au.app_user_email,
                    m.member_account,
                    m.member_nickname,
                    m.member_gamemoney
                FROM mg_app_user au
                INNER JOIN mg_member m ON m.member_id = au.app_user_member_id
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
    }
}
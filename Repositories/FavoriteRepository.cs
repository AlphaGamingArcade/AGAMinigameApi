using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IFavoriteRepository
    {
        Task<(List<Favorite> items, int total)> GetPaginatedFavoritesByMemberIdAsync(
            int memberId,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize);
        // Task<Banner> GetById(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }

    public class FavoriteRepository : BaseRepository, IFavoriteRepository
    {
        public FavoriteRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<(List<Favorite> items, int total)> GetPaginatedFavoritesByMemberIdAsync(
            int memberId,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var items = new List<Favorite>();
            int offset = (pageNumber - 1) * pageSize;

            // Whitelist sortable columns
            string orderColumn = sortBy?.ToLowerInvariant() switch
            {
                "id" => "b.betting_id",
                "memberid" => "b.betting_member_id",
                "gamecodeid" => "b.betting_gamecode_id",
                "agentid" => "b.betting_agent_id",
                "money" => "b.betting_money",
                "benefit" => "b.betting_benefit",
                "result" => "b.betting_result",
                "datetime" => "b.betting_datetime",
                // gamecode fields
                "code" => "gc.gamecode_code",
                "name" => "gc.gamecode_name",
                "order" => "gc.gamecode_order",
                _ => "b.betting_id"
            };
            string orderDir = descending ? "DESC" : "ASC";

            // total count (filtered)
            const string countSql = @"SELECT COUNT(1) AS TotalCount FROM mg_betting WHERE betting_member_id = @memberId;";
            var countParams = new Dictionary<string, object> { { "@memberId", memberId } };
            DataTable countTable = await SelectQueryAsync(countSql, countParams);
            int total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // page query (always join gamecode)
            string pageSql = $@"
                SELECT 
                    b.betting_id,
                    b.betting_member_id,
                    b.betting_gamecode_id,
                    b.betting_agent_id,
                    b.betting_money,
                    b.betting_benefit,
                    b.betting_result,
                    b.betting_datetime,
                    gc.gamecode_id                      AS gc_id,
                    gc.gamecode_code                    AS gc_code,
                    gc.gamecode_name                    AS gc_name,
                    gc.gamecode_name_multi_language     AS gc_name_multi_language,
                    gc.gamecode_percent                 AS gc_percent,
                    gc.gamecode_datetime                AS gc_datetime,
                    gc.gamecode_status                  AS gc_status,
                    gc.gamecode_order                   AS gc_order,
                    gc.gamecode_game_type               AS gc_game_type
                FROM mg_betting b
                LEFT JOIN mg_gamecode gc 
                    ON gc.gamecode_id = b.betting_gamecode_id
                WHERE b.betting_member_id = @memberId
                ORDER BY {orderColumn} {orderDir}
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            var pageParams = new Dictionary<string, object>
                {
                    { "@memberId", memberId },
                    { "@offset", offset },
                    { "@pageSize", pageSize }
                };

            DataTable pageTable = await SelectQueryAsync(pageSql, pageParams);
            foreach (DataRow row in pageTable.Rows)
                items.Add(row.ToFavoriteFromDataRow());

            return (items, total);
        }
    }
}
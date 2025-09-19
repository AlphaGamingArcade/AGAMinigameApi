using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IBettingRepository
    {
        Task<(List<Betting> items, int total)> GetPaginatedBettingsByMemberIdAsync(
            int memberId,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize,
            char? gameType);
        // Task<Banner> GetById(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }

    public class BettingRepository : BaseRepository, IBettingRepository
    {
        public BettingRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<(List<Betting> items, int total)> GetPaginatedBettingsByMemberIdAsync(
            int memberId,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize,
            char? gameType
        )
        {
            // Normalize paging
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 200);
            int offset = (pageNumber - 1) * pageSize;

            // Whitelist sortable columns
            string orderColumn = (sortBy ?? "").ToLowerInvariant() switch
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

            // WHERE parts (always filter by member; optionally by gameType)
            var whereParts = new List<string> { "b.betting_member_id = @memberId" };
            var parameters = new Dictionary<string, object> { ["@memberId"] = memberId };

            if (gameType.HasValue)
            {
                whereParts.Add("gc.gamecode_game_type = @gameType");
                parameters["@gameType"] = gameType.Value;
            }

            string whereClause = "WHERE " + string.Join(" AND ", whereParts);

            // COUNT (join because we may filter by gameType)
            string countSql = $@"
                SELECT COUNT(1) AS TotalCount
                FROM mg_betting b
                LEFT JOIN mg_gamecode gc ON gc.gamecode_id = b.betting_gamecode_id
                {whereClause};";

            DataTable countTable = await SelectQueryAsync(countSql, parameters);
            int total = countTable.Rows.Count > 0
                ? Convert.ToInt32(countTable.Rows[0]["TotalCount"])
                : 0;

            // PAGE
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

                    gc.gamecode_id,
                    gc.gamecode_code,
                    gc.gamecode_name,
                    gc.gamecode_name_multi_language,
                    gc.gamecode_percent,
                    gc.gamecode_datetime,
                    gc.gamecode_status,
                    gc.gamecode_order,
                    gc.gamecode_game_type
                FROM mg_betting b
                LEFT JOIN mg_gamecode gc ON gc.gamecode_id = b.betting_gamecode_id
                {whereClause}
                ORDER BY {orderColumn} {orderDir}
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            parameters["@offset"] = offset;
            parameters["@pageSize"] = pageSize;

            var items = new List<Betting>();
            DataTable pageTable = await SelectQueryAsync(pageSql, parameters);
            foreach (DataRow row in pageTable.Rows)
                items.Add(row.ToBettingFromDataRow());

            return (items, total);
        }
    }
}
using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IPlayRepository
    {
        Task<(List<Game> items, int total)> GetPaginatedPlaysByMemberIdAsync(
            int memberId,
            string? search,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize,
            char? gameType = null,
            bool? top = null,
            bool? trending = null,
            bool? latest = null);
        // Task<Banner> GetById(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }

    public class PlayRepository : BaseRepository, IPlayRepository
    {
        public PlayRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<(List<Game> items, int total)> GetPaginatedPlaysByMemberIdAsync(
            int memberId,
            string? search,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize,
            char? gameType = null,
            bool? top = null,
            bool? trending = null,
            bool? latest = null
        )
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var items = new List<Game>();
            int offset = (pageNumber - 1) * pageSize;

            // Whitelist sortable columns
            string orderColumn = (sortBy ?? "").ToLowerInvariant() switch
            {
                "memberid" => "ap.app_play_member_id",
                "gamecode" => "ap.app_play_game_code",
                "createdat" => "ap.app_play_created_at",
                "updatedat" => "ap.app_play_updated_at",
                _ => "ap.app_play_created_at"
            };
            string orderDir = descending ? "DESC" : "ASC";

            // Build WHERE conditions
            var whereConditions = new List<string> { "ap.app_play_member_id = @memberId" };
            var queryParams = new Dictionary<string, object> { { "@memberId", memberId } };

            // Search condition
            if (!string.IsNullOrEmpty(search))
            {
                whereConditions.Add(@"
                (
                    gc.gamecode_name LIKE CONCAT('%', @search, '%') OR
                    ag.game_code LIKE CONCAT('%', @search, '%') OR
                    ag.game_category LIKE CONCAT('%', @search, '%') OR
                    ag.game_description LIKE CONCAT('%', @search, '%')
                )");
                queryParams["@search"] = search;
            }

            // Game type filter
            if (gameType.HasValue)
            {
                whereConditions.Add("gc.gamecode_game_type = @gameType");
                queryParams["@gameType"] = gameType.Value;
            }

            // Boolean filters
            if (top.HasValue)
            {
                whereConditions.Add("ag.game_top = @top");
                queryParams["@top"] = top.Value ? "y" : "n";
            }

            if (trending.HasValue)
            {
                whereConditions.Add("ag.game_trending = @trending");
                queryParams["@trending"] = trending.Value ? "y" : "n";
            }

            if (latest.HasValue)
            {
                whereConditions.Add("ag.game_latest = @latest");
                queryParams["@latest"] = latest.Value ? "y" : "n";
            }

            string whereClause = string.Join(" AND ", whereConditions);

            // Total count query (with all filters)
            string countSql = $@"
                SELECT COUNT(1) AS TotalCount
                FROM mg_app_play ap
                INNER JOIN mg_gamecode gc ON gc.gamecode_code = ap.app_play_game_code
                INNER JOIN mg_app_game ag ON ag.game_code = gc.gamecode_code
                WHERE {whereClause};";

            DataTable countTable = await SelectQueryAsync(countSql, queryParams);
            int total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // Page query with all filters
            string pageSql = $@"
                SELECT 
                    ap.app_play_member_id,
                    ap.app_play_game_code,
                    ap.app_play_created_at,
                    ap.app_play_updated_at,
                    -- mg_app_game (for mapper: game_*)
                    ag.game_code,
                    ag.game_description,
                    ag.game_description_multi_language,
                    ag.game_image,
                    ag.game_play_url,
                    ag.game_status,
                    ag.game_category,
                    ag.game_top,
                    ag.game_latest,
                    ag.game_trending,
                    ag.game_datetime,
                    -- mg_gamecode (for mapper: gamecode_*)
                    gc.gamecode_id,
                    gc.gamecode_code,
                    gc.gamecode_name,
                    gc.gamecode_name_multi_language,
                    gc.gamecode_percent,
                    gc.gamecode_datetime,
                    gc.gamecode_status,
                    gc.gamecode_order,
                    gc.gamecode_game_type
                FROM mg_app_play ap
                INNER JOIN mg_gamecode gc ON gc.gamecode_code = ap.app_play_game_code
                INNER JOIN mg_app_game ag ON ag.game_code = gc.gamecode_code
                WHERE {whereClause}
                ORDER BY {orderColumn} {orderDir}
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            var pageParams = new Dictionary<string, object>(queryParams)
            {
                { "@offset", offset },
                { "@pageSize", pageSize }
            };

            DataTable pageTable = await SelectQueryAsync(pageSql, pageParams);
            foreach (DataRow row in pageTable.Rows)
                items.Add(row.ToGameFromDataRow());

            return (items, total);
        }
    }
}
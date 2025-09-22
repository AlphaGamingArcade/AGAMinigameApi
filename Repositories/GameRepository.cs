using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IGameRepository
    {
        Task<(List<Game> items, int total)> GetPaginatedGamesAsync(
            string? search,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize,
            char? gameType,
            bool? top,
            bool? trending,
            bool? latest
        );
        Task<Game?> GetByIdAsync(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }

    public class GameRepository : BaseRepository, IGameRepository
    {
        public GameRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<Game?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    gc.gamecode_id,
                    gc.gamecode_code,
                    gc.gamecode_name,
                    gc.gamecode_name_multi_language,
                    ag.game_code,
                    ag.game_description,
                    ag.game_description_multi_language,
                    ag.game_image,
                    ag.game_url,
                    ag.game_status,
                    ag.game_category,
                    ag.game_top,
                    ag.game_trending,
                    ag.game_datetime
                FROM mg_app_game ag
                INNER JOIN mg_gamecode gc ON gc.gamecode_code = ag.game_code
                WHERE gc.gamecode_id = @id;";

            var dt = await SelectQueryAsync(sql, new Dictionary<string, object> { { "@id", id } });
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return row.ToGameFromDataRow();
        }

        public async Task<(List<Game> items, int total)> GetPaginatedGamesAsync(
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
            var items = new List<Game>();

            // Normalize paging
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 200);
            int offset = (pageNumber - 1) * pageSize;

            // Whitelist sortable columns
            string orderColumn = (sortBy ?? "").ToLowerInvariant() switch
            {
                "name" => "gc.gamecode_name",
                "datetime" => "ag.game_datetime",
                "code" => "ag.game_code",
                "category" => "ag.game_category",
                _ => "gc.gamecode_id"
            };
            string orderDir = descending ? "DESC" : "ASC";

            // Build WHERE parts
            var whereParts = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (gameType != null)
            {
                whereParts.Add("gc.gamecode_game_type = @gameType");
                parameters["@gameType"] = gameType;
            }

            if (top.HasValue &&  top is true)
            {
                whereParts.Add("ag.game_top = @top");
                parameters["@top"] = 'y';
            }

            if (trending.HasValue && trending is true)
            {
                whereParts.Add("ag.game_trending = @trending");
                parameters["@trending"] = 'y';
            }

            if (latest.HasValue && latest is true)
            {
                whereParts.Add("ag.game_latest = @latest");
                parameters["@latest"] = 'y';
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                whereParts.Add(@"
                (
                    gc.gamecode_name    LIKE CONCAT('%', @search, '%') OR
                    ag.game_code        LIKE CONCAT('%', @search, '%') OR
                    ag.game_category    LIKE CONCAT('%', @search, '%') OR
                    ag.game_description LIKE CONCAT('%', @search, '%')
                )");
                parameters["@search"] = search;
            }

            string whereClause = whereParts.Count > 0 ? "WHERE " + string.Join(" AND ", whereParts) : "";

            // Count
            string countSql = $@"
                SELECT COUNT(1) AS TotalCount
                FROM mg_app_game ag
                INNER JOIN mg_gamecode gc ON gc.gamecode_code = ag.game_code
                {whereClause};";

            DataTable countTable = await SelectQueryAsync(countSql, parameters);
            int total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // Page
            string pageSql = $@"
                SELECT 
                    gc.gamecode_id,
                    gc.gamecode_code,
                    gc.gamecode_name,
                    gc.gamecode_name_multi_language,
                    ag.game_code,
                    ag.game_description,
                    ag.game_description_multi_language,
                    ag.game_image,
                    ag.game_url,
                    ag.game_status,
                    ag.game_category,
                    ag.game_top,
                    ag.game_trending,
                    ag.game_datetime
                FROM mg_app_game ag
                INNER JOIN mg_gamecode gc ON gc.gamecode_code = ag.game_code
                {whereClause}
                ORDER BY {orderColumn} {orderDir}
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            parameters["@offset"] = offset;
            parameters["@pageSize"] = pageSize;

            DataTable pageTable = await SelectQueryAsync(pageSql, parameters);
            foreach (DataRow row in pageTable.Rows)
                items.Add(row.ToGameFromDataRow());

            return (items, total);
        }
    }
}
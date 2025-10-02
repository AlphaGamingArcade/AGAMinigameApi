using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IFavoriteRepository
    {
        Task<(List<Game> items, int total)> GetPaginatedFavoritesByMemberIdAsync(
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
        );
        Task<Favorite> AddAsync(Favorite favorite);
        Task<bool> ExistsAsync(int memberId, int gameId);
        Task<Game?> GetFavoriteByMemberIdAndGameIdAsync(int memberId, int gameId);
        Task<int> DeleteByMemberIdAndGameIdAsync(int memberId, int gameId);
        // Task<Banner> GetById(int id);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }

    public class FavoriteRepository : BaseRepository, IFavoriteRepository
    {
        public FavoriteRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<bool> ExistsAsync(int memberId, int gameId)
        {
            const string sql = @"
                SELECT TOP 1 1
                FROM mg_favorite
                WHERE favorite_member_id = @memberId
                AND favorite_game_id   = @gameId;";

            var parameters = new Dictionary<string, object>
            {
                { "@memberId", memberId },
                { "@gameId", gameId }
            };

            var dt = await SelectQueryAsync(sql, parameters);
            return dt.Rows.Count > 0;
        }

        public async Task<Game?> GetFavoriteByMemberIdAndGameIdAsync(int memberId, int gameId)
        {
            const string sql = @"
                 SELECT TOP (1)
                -- favorite
                f.favorite_id,
                f.favorite_member_id,
                f.favorite_game_id,
                f.favorite_created_at,
                f.favorite_updated_at,

                -- mg_game (aliased to game_*)
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

                -- mg_gamecode (kept as gamecode_*)
                gc.gamecode_id,
                gc.gamecode_code,
                gc.gamecode_name,
                gc.gamecode_name_multi_language,
                gc.gamecode_percent,
                gc.gamecode_datetime,
                gc.gamecode_status,
                gc.gamecode_order,
                gc.gamecode_game_type
            FROM dbo.mg_favorite AS f
            JOIN dbo.mg_gamecode AS gc ON gc.gamecode_id = f.favorite_game_id
            JOIN dbo.mg_game AS ag ON ag.game_code = gc.gamecode_code
            WHERE f.favorite_member_id = @memberId AND f.favorite_game_id   = @gameId
            ORDER BY f.favorite_created_at DESC, f.favorite_id DESC;";

            var parameters = new Dictionary<string, object>
            {
                { "@memberId", memberId },
                { "@gameId", gameId }
            };

            var dt = await SelectQueryAsync(sql, parameters);
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return row.ToGameFromDataRow();
        }

        public async Task<int> DeleteByMemberIdAndGameIdAsync(int memberId, int gameId)
        {
            const string query = @"
                DELETE FROM mg_favorite
                WHERE favorite_member_id = @memberId
                AND favorite_game_id = @gameId;";

            var parameters = new Dictionary<string, object>
            {
                ["@memberId"] = memberId,
                ["@gameId"] = gameId
            };

            return await DeleteQueryAsync(query, parameters);
        }

        public async Task<Favorite> AddAsync(Favorite favorite)
        {
            const string query = @"
                INSERT INTO mg_favorite
                    (favorite_member_id, favorite_game_id, favorite_created_at, favorite_updated_at)
                OUTPUT
                    INSERTED.favorite_id,
                    INSERTED.favorite_member_id,
                    INSERTED.favorite_game_id,
                    INSERTED.favorite_created_at,
                    INSERTED.favorite_updated_at
                VALUES
                    (@memberId, @gameId, @createdAt, @updatedAt);
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@memberId", favorite.MemberId },
                { "@gameId", favorite.GameId },
                { "@createdAt", favorite.CreatedAt == default ? DateTime.UtcNow : favorite.CreatedAt },
                { "@updatedAt", favorite.UpdatedAt.HasValue ? favorite.UpdatedAt.Value : DBNull.Value }
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            if (dataTable.Rows.Count == 0)
                throw new Exception("Failed to insert favorite.");

            var row = dataTable.Rows[0];
            return row.ToFavoriteFromDataRow();
        }

        public async Task<(List<Game> items, int total)> GetPaginatedFavoritesByMemberIdAsync(
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
                "id" => "f.favorite_id",
                "memberid" => "f.favorite_member_id",
                "itemid" => "f.favorite_game_id",
                "createdat" => "f.favorite_created_at",
                "updatedat" => "f.favorite_updated_at",
                _ => "f.favorite_created_at"
            };
            string orderDir = descending ? "DESC" : "ASC";

            // Build WHERE conditions
            var whereConditions = new List<string> { "f.favorite_member_id = @memberId" };
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
                FROM mg_favorite f
                INNER JOIN mg_gamecode gc ON gc.gamecode_id = f.favorite_game_id
                INNER JOIN mg_game ag ON ag.game_code = gc.gamecode_code
                WHERE {whereClause};";

            DataTable countTable = await SelectQueryAsync(countSql, queryParams);
            int total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // Page query with all filters
            string pageSql = $@"
                SELECT 
                    f.favorite_id,
                    f.favorite_member_id,
                    f.favorite_game_id,
                    f.favorite_created_at,
                    f.favorite_updated_at,
                    -- mg_game (for mapper: game_*)
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
                FROM mg_favorite f
                INNER JOIN mg_gamecode gc ON gc.gamecode_id = f.favorite_game_id
                INNER JOIN mg_game ag ON ag.game_code = gc.gamecode_code
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
using System.Data;
using AGAMinigameApi.Dtos.Play;
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
        Task<bool> ExistsAsync(int memberId, int gameId);
        Task<Play?> GetPlayByMemberIdAndGameIdAsync(int memberId, int gameId);
        Task<Play> AddAsync(Play play);
        Task<Play> UpdateByMemberIdAndGameIdAsync(int memberId, int gameId, DateTime dateTime);
        Task<Play> UpdateAsync(Play play);
    }

    public class PlayRepository : BaseRepository, IPlayRepository
    {
        public PlayRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<bool> ExistsAsync(int memberId, int gameId)
        {
            const string sql = @"
                SELECT TOP 1 1
                FROM mg_play
                WHERE play_member_id = @memberId
                AND play_game_id = @gameId;";

            var parameters = new Dictionary<string, object>
            {
                { "@memberId", memberId },
                { "@gameId", gameId }
            };

            var dt = await SelectQueryAsync(sql, parameters);
            return dt.Rows.Count > 0;
        }

        public async Task<Play> AddAsync(Play play)
        {
            const string query = @"
                INSERT INTO mg_play
                    (play_member_id, play_game_id, play_created_at, play_updated_at)
                OUTPUT
                    INSERTED.play_id,
                    INSERTED.play_member_id,
                    INSERTED.play_game_id,
                    INSERTED.play_created_at,
                    INSERTED.play_updated_at
                VALUES
                    (@memberId, @gameId, @createdAt, @updatedAt);
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@memberId", play.MemberId },
                { "@gameId", play.GameId },
                { "@createdAt", play.CreatedAt},
                { "@updatedAt", play.UpdatedAt }
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            if (dataTable.Rows.Count == 0)
                throw new Exception("Failed to insert play record.");

            var row = dataTable.Rows[0];
            return row.ToPlayFromDataRow();
        }

        public async Task<Play> UpdateAsync(Play play)
        {
            const string query = @"
                UPDATE mg_play
                SET 
                    play_member_id = @memberId,
                    play_game_id = @gameId,
                    play_updated_at = @updatedAt
                OUTPUT
                    INSERTED.play_id,
                    INSERTED.play_member_id,
                    INSERTED.play_game_id,
                    INSERTED.play_created_at,
                    INSERTED.play_updated_at
                WHERE 
                    play_id = @playId;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@playId", play.Id },
                { "@memberId", play.MemberId },
                { "@gameId", play.GameId },
                { "@updatedAt", play.UpdatedAt }
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            if (dataTable.Rows.Count == 0)
                throw new Exception("Failed to update play record or record not found.");

            var row = dataTable.Rows[0];
            return row.ToPlayFromDataRow();
        }

        public async Task<Play> UpdateByMemberIdAndGameIdAsync(int memberId, int gameId, DateTime updatedAt)
        {
            const string query = @"
                UPDATE mg_play
                SET 
                    play_updated_at = @updatedAt
                OUTPUT
                    INSERTED.play_id,
                    INSERTED.play_member_id,
                    INSERTED.play_game_id,
                    INSERTED.play_created_at,
                    INSERTED.play_updated_at
                WHERE 
                    play_member_id = @memberId 
                    AND play_game_id = @gameId;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@memberId", memberId },
                { "@gameId", gameId },
                { "@updatedAt", updatedAt }  // Timestamp provided by service
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            if (dataTable.Rows.Count == 0)
                throw new Exception("Failed to update play record or record not found.");

            var row = dataTable.Rows[0];
            return row.ToPlayFromDataRow();
        }

        public async Task<Play?> GetPlayByMemberIdAndGameIdAsync(int memberId, int gameId)
        {
            const string sql = @"
                SELECT TOP 1
                    ap.play_id,
                    ap.play_member_id,
                    ap.play_game_id,
                    ap.play_created_at,
                    ap.play_updated_at,
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
                FROM mg_play ap
                INNER JOIN mg_gamecode gc ON gc.gamecode_id = ap.play_game_id
                INNER JOIN mg_game ag ON ag.game_code = gc.gamecode_code
                WHERE ap.play_member_id = @memberId
                AND ap.play_game_id = @gameId;";

            var parameters = new Dictionary<string, object>
            {
                { "@memberId", memberId },
                { "@gameId", gameId }
            };

            var dt = await SelectQueryAsync(sql, parameters);
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return row.ToPlayFromDataRow();
        }
        
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
                "memberid" => "ap.play_member_id",
                "gameid" => "ap.play_game_id",
                "createdat" => "ap.play_created_at",
                "updatedat" => "ap.play_updated_at",
                _ => "ap.play_created_at"
            };
            string orderDir = descending ? "DESC" : "ASC";

            // Build WHERE conditions
            var whereConditions = new List<string> { "ap.play_member_id = @memberId" };
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
                FROM mg_play ap
                INNER JOIN mg_gamecode gc ON gc.gamecode_id = ap.play_game_id
                INNER JOIN mg_game ag ON ag.game_code = gc.gamecode_code
                WHERE {whereClause};";

            DataTable countTable = await SelectQueryAsync(countSql, queryParams);
            int total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // Page query with all filters
            string pageSql = $@"
                SELECT
                    ap.play_id,
                    ap.play_member_id,
                    ap.play_game_id,
                    ap.play_created_at,
                    ap.play_updated_at,
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
                FROM mg_play ap
                INNER JOIN mg_gamecode gc ON gc.gamecode_id = ap.play_game_id
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
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
        Task<Favorite> AddAsync(Favorite favorite);
        Task<bool> ExistsAsync(int memberId, int gameId, string gameType);
        Task<Favorite?> GetFavoriteByMemberIdAndGameIdAsync(int memberId, int gameId);
        // Task<Banner> GetById(int id);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }

    public class FavoriteRepository : BaseRepository, IFavoriteRepository
    {
        public FavoriteRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<bool> ExistsAsync(int memberId, int gameId, string gameType)
        {
            const string sql = @"
                SELECT TOP 1 1
                FROM mg_favorite
                WHERE favorite_member_id = @memberId
                AND favorite_game_id   = @gameId;";

            var parameters = new Dictionary<string, object>
            {
                { "@memberId", memberId },
                { "@gameType", gameType },    // expect 1-char string
                { "@gameId", gameId }
            };

            var dt = await SelectQueryAsync(sql, parameters);
            return dt.Rows.Count > 0;
        }

        public async Task<Favorite?> GetFavoriteByMemberIdAndGameIdAsync(int memberId, int gameId)
        {
            const string sql = @"
                SELECT TOP 1
                    f.favorite_id,
                    f.favorite_member_id,
                    f.favorite_game_id,
                    f.favorite_created_at,
                    f.favorite_updated_at
                FROM mg_favorite f
                WHERE f.favorite_member_id = @memberId
                AND f.favorite_game_id   = @gameId;";

            var parameters = new Dictionary<string, object>
            {
                { "@memberId", memberId },
                { "@gameId", gameId }
            };

            var dt = await SelectQueryAsync(sql, parameters);
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return row.ToFavoriteFromDataRow();
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
                "id" => "f.favorite_id",
                "memberid" => "f.favorite_member_id",
                "itemid" => "f.favorite_game_id",
                "createdat" => "f.favorite_created_at", 
                "updatedat" => "f.favorite_updated_at",
                _ => "f.favorite_id"
            };
            string orderDir = descending ? "DESC" : "ASC";

            // total count (filtered)
            const string countSql = @"SELECT COUNT(1) AS TotalCount 
                              FROM mg_favorite 
                              WHERE favorite_member_id = @memberId;";
            var countParams = new Dictionary<string, object> { { "@memberId", memberId } };
            DataTable countTable = await SelectQueryAsync(countSql, countParams);
            int total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // page query
            string pageSql = $@"
                SELECT 
                    f.favorite_id,
                    f.favorite_member_id,
                    f.favorite_game_id,
                    f.favorite_created_at,
                    f.favorite_updated_at
                FROM mg_favorite f
                WHERE f.favorite_member_id = @memberId
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
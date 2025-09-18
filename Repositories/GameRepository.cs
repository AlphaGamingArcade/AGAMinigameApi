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
            int pageSize
        );
        // Task<Banner> GetById(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }

    public class GameRepository : BaseRepository, IGameRepository
    {
        public GameRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<IEnumerable<Game>> GetAll()
        {
            var games = new List<Game>();
            var table = await SelectQueryAsync("SELECT * FROM mg_app_game");
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    games.Add(row.ToGameFromDataRow());
                }
                return games;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(List<Game> items, int total)> GetPaginatedGamesAsync(
            string? search,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize)
        {
            var items = new List<Game>();

            // Whitelist sortable columns to avoid ORDER BY injection
            string orderColumn = sortBy?.ToLowerInvariant() switch
            {
                "name" => "game_name",
                "datetime" => "game_datetime",
                "code" => "game_code",
                "category" => "game_category",
                _ => "game_id"
            };
            string orderDir = descending ? "DESC" : "ASC";

            int offset = Math.Max(0, (pageNumber - 1) * pageSize);

            // Build WHERE condition for search
            string whereClause = "";
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                whereClause = @"
                WHERE 
                    game_name LIKE @search OR 
                    game_code LIKE @search OR 
                    game_category LIKE @search OR
                    game_description LIKE @search";
                    parameters["@search"] = $"%{search}%";
            }

            // Count query
            string countSql = $@"SELECT COUNT(1) AS TotalCount 
                         FROM mg_app_game
                         {whereClause};";

            DataTable countTable = await SelectQueryAsync(countSql, parameters);
            int total = countTable.Rows.Count > 0
                ? Convert.ToInt32(countTable.Rows[0]["TotalCount"])
                : 0;

            // Page query
            string pageSql = $@"
                SELECT 
                    game_id, game_code, game_name, game_description, game_image, game_url,
                    game_status, game_top, game_trending, game_datetime
                FROM mg_app_game
                {whereClause}
                ORDER BY {orderColumn} {orderDir}
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            parameters["@offset"] = offset;
            parameters["@pageSize"] = pageSize;

            DataTable pageTable = await SelectQueryAsync(pageSql, parameters);
            foreach (DataRow row in pageTable.Rows)
            {
                items.Add(row.ToGameFromDataRow());
            }

            return (items, total);
        }
    }
}
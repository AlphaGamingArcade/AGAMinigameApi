using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IGameRepository
    {
        Task<(List<Game> items, int total)> GetPaginatedGamesAsync(
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
            var table = await SelectQueryAsync("SELECT * FROM mg_game");
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
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize)
        {
            // Manually map DataTable rows to a List<Game>
            var items = new List<Game>();

            // Whitelist sortable columns to avoid ORDER BY injection
            string orderColumn = sortBy?.ToLowerInvariant() switch
            {
                "name" => "game_name",
                "createdat" => "game_created_at",
                "code" => "game_code",
                "category" => "game_category",
                _ => "game_id"
            };
            string orderDir = descending ? "DESC" : "ASC";

            int offset = Math.Max(0, (pageNumber - 1) * pageSize);

            // Get total count separately using SelectQueryAsync
            const string countSql = @"SELECT COUNT(1) AS TotalCount FROM mg_game;";
            DataTable countTable = await SelectQueryAsync(countSql);
            int total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // Get paginated data using SelectQueryAsync
            string pageSql = $@"
                SELECT 
                    game_id, game_code, game_name, game_description, game_image, game_url,
                    game_status, game_top, game_trending, game_created_at
                FROM mg_game
                ORDER BY {orderColumn} {orderDir}
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            var pageParameters = new Dictionary<string, object>
            {
                { "@offset", offset },
                { "@pageSize", pageSize }
            };

            DataTable pageTable = await SelectQueryAsync(pageSql, pageParameters);
            foreach (DataRow row in pageTable.Rows)
            {
                items.Add(row.ToGameFromDataRow());
            }

            return (items, total);
        }
    }
}
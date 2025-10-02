using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IGamePreviewRepository
    {
        Task<(List<GamePreview> items, int total)> GetPaginatedGamePreviewsAsync(
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize);
    }

    public class GamePreviewRepository : BaseRepository, IGamePreviewRepository
    {
        public GamePreviewRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<(List<GamePreview> items, int total)> GetPaginatedGamePreviewsAsync(
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize)
        {
            var items = new List<GamePreview>();

            string orderColumn = sortBy?.ToLowerInvariant() switch
            {
                "title" => "game_preview_title",
                "datetime" => "game_preview_datetime",
                "order" => "game_preview_order",
                _ => "game_preview_id"
            };
            string orderDir = descending ? "DESC" : "ASC";

            int offset = Math.Max(0, (pageNumber - 1) * pageSize);

            const string countSql = @"SELECT COUNT(1) AS TotalCount FROM mg_game_preview;";
            DataTable countTable = await SelectQueryAsync(countSql);
            int total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            string pageSql = $@"
            SELECT 
                game_preview_id,
                game_preview_game_id,
                game_preview_image,
                game_preview_title,
                game_preview_description,
                game_preview_order,
                game_preview_datetime
            FROM mg_game_preview
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
                items.Add(row.ToGamePreviewFromDataRow());
            }

            return (items, total);
        }
    }
}
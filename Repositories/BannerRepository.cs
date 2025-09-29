using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IBannerRepository
    {
        Task<IEnumerable<Banner>> GetAll();
        Task<(List<Banner> items, int total)> GetPaginatedBannersAsync(
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize);
        // Task<Banner> GetById(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }

    public class BannerRepository : BaseRepository, IBannerRepository
    {
        public BannerRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<IEnumerable<Banner>> GetAll()
        {
            var banners = new List<Banner>();
            var table = await SelectQueryAsync("SELECT * FROM mg_app_banner");
            foreach (DataRow row in table.Rows)
            {
                banners.Add(row.ToBannerFromDataRow());
            }
            return banners;
        }

    public async Task<(List<Banner> items, int total)> GetPaginatedBannersAsync(
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize)
        {
            // Manually map DataTable rows to a List<Game>
            var items = new List<Banner>();

            // Whitelist sortable columns to avoid ORDER BY injection
            string orderColumn = sortBy?.ToLowerInvariant() switch
            {
                "title" => "banner_title",
                "datetime" => "banner_datetime",
                "order" => "banner_order",
                _ => "banner_id"
            };
            string orderDir = descending ? "DESC" : "ASC";

            int offset = Math.Max(0, (pageNumber - 1) * pageSize);

            // Get total count separately using SelectQueryAsync
            const string countSql = @"SELECT COUNT(1) AS TotalCount FROM mg_app_banner;";
            DataTable countTable = await SelectQueryAsync(countSql);
            int total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // Get paginated data using SelectQueryAsync
            string pageSql = $@"
                SELECT 
                    banner_id, banner_title, banner_description, banner_image, banner_url, banner_order, banner_datetime
                FROM mg_app_banner
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
                items.Add(row.ToBannerFromDataRow());
            }

            return (items, total);
        }
    }
}
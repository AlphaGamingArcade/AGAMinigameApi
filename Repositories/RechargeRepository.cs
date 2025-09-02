using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IRechargeRepository
    {
        Task<(List<Recharge> items, int total)> GetPaginatedRechargesAsync(
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize);
        Task<(List<Recharge> items, int total)> GetPaginatedRechargesByMemberIdAsync(
            int memberId,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize);
        // Task<Banner> GetById(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }

    public class RechargeRepository : BaseRepository, IRechargeRepository
    {
        public RechargeRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<IEnumerable<Game>> GetAll()
        {
            var games = new List<Game>();
            var table = await SelectQueryAsync("SELECT * FROM mg_recharge");
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

        public async Task<(List<Recharge> items, int total)> GetPaginatedRechargesByMemberIdAsync(
            int memberId,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize)
        {
            var items = new List<Recharge>();

            // Whitelist sort columns
            var orderColumn = (sortBy ?? "").ToLowerInvariant() switch
            {
                "date" => "recharge_date",
                "datetime" => "recharge_datetime",
                _ => "recharge_id"
            };
            var orderDir = descending ? "DESC" : "ASC";

            var safePage = Math.Max(1, pageNumber);
            var safeSize = Math.Clamp(pageSize, 1, 200);
            var offset = (safePage - 1) * safeSize;

            // Count (filtered)
            const string countSql = @"
                SELECT COUNT(1) AS TotalCount
                FROM mg_recharge
                WHERE recharge_member_id = @memberId;";
            var countParams = new Dictionary<string, object> { ["@memberId"] = memberId };
            var countTable = await SelectQueryAsync(countSql, countParams);
            var total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // Build ORDER BY safely (avoid duplicate column)
            var orderBySql = orderColumn == "recharge_id"
                ? $"ORDER BY recharge_id {orderDir}"
                : $"ORDER BY {orderColumn} {orderDir}, recharge_id ASC";

            // Page (filtered)
            var pageSql = $@"
                SELECT recharge_id, recharge_member_id, recharge_agent_id,
                    recharge_gamemoney, recharge_currency, recharge_date, recharge_datetime
                FROM mg_recharge
                WHERE recharge_member_id = @memberId
                {orderBySql}
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            var pageParams = new Dictionary<string, object>
            {
                ["@memberId"] = memberId,
                ["@offset"] = offset,
                ["@pageSize"] = safeSize
            };

            var pageTable = await SelectQueryAsync(pageSql, pageParams);
            foreach (DataRow row in pageTable.Rows)
                items.Add(row.ToRechargeFromDataRow());

            return (items, total);
        }
        public async Task<(List<Recharge> items, int total)> GetPaginatedRechargesAsync(
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize)
        {
            // Manually map DataTable rows to a List<Game>
            var items = new List<Recharge>();

            // Whitelist sortable columns to avoid ORDER BY injection
            string orderColumn = sortBy?.ToLowerInvariant() switch
            {
                "date" => "recharge_date",
                "datetime" => "recharge_datetime",
                _ => "recharge_id"
            };
            string orderDir = descending ? "DESC" : "ASC";

            int offset = Math.Max(0, (pageNumber - 1) * pageSize);

            // Get total count separately using SelectQueryAsync
            const string countSql = @"SELECT COUNT(1) AS TotalCount FROM mg_recharge;";
            DataTable countTable = await SelectQueryAsync(countSql);
            int total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // Get paginated data using SelectQueryAsync
            string pageSql = $@"
                SELECT 
                    recharge_id, recharge_member_id, recharge_agent_id, recharge_gamemoney, recharge_currency, recharge_date, recharge_datetime
                FROM mg_recharge
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
                items.Add(row.ToRechargeFromDataRow());
            }

            return (items, total);
        }
    }
}
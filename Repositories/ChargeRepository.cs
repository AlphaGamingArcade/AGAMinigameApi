using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    // Task<Banner> GetById(int id);
    // Task<int> Add(Banner banner);
    // Task<int> Update(Banner banner);
    // Task<int> Delete(int id);

    public interface IChargeRepository
    {
        Task<bool> IsChargeExistsAsync(int memberId, DateTime datetime);
        Task<Charge> AddAsync(Charge charge);
        Task<(List<Charge> items, int total)> GetPaginatedChargesAsync(
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize);
        Task<(List<Charge> items, int total)> GetPaginatedChargesByMemberIdAsync(
            int memberId,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize);
    }

    public class ChargeRepository : BaseRepository, IChargeRepository
    {
        public ChargeRepository(IConfiguration configuration) : base(configuration) { }


        public async Task<bool> IsChargeExistsAsync(int memberId, DateTime datetime)
        {
            const string query = @"SELECT charge_id FROM mg_charge where charge_date = @date AND charge_member_id = @memberId";
            var parameters = new Dictionary<string, object>
            {
                { "@date", DateOnly.FromDateTime(datetime)  },
                { "@memberId", memberId }
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            return dataTable.Rows.Count > 0;
        }

        public async Task<Charge> AddAsync(Charge charge)
        {
            const string query = @"
                INSERT INTO mg_charge
                    (charge_member_id, charge_agent_id, charge_gamemoney, charge_currency, charge_date, charge_datetime)
                OUTPUT INSERTED.charge_id,
                    INSERTED.charge_member_id,
                    INSERTED.charge_agent_id,
                    INSERTED.charge_gamemoney,
                    INSERTED.charge_currency,
                    INSERTED.charge_date,
                    INSERTED.charge_datetime
                VALUES
                    (@memberId, @agentId, @gamemoney, @currency, @date, @datetime);
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@memberId", charge.MemberId },
                { "@agentId", charge.AgentId },
                { "@gamemoney", charge.Gamemoney },
                { "@currency", charge.Currency },
                { "@date", charge.Date },
                { "@datetime", charge.Datetime }
            };

            var dataTable = await SelectQueryAsync(query, parameters);

            if (dataTable.Rows.Count == 0)
                throw new Exception("Failed to insert charge.");

            var row = dataTable.Rows[0];
            return row.ToChargeFromDataRow();
        }

        public async Task<(List<Charge> items, int total)> GetPaginatedChargesByMemberIdAsync(
            int memberId,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize)
        {
            var items = new List<Charge>();

            // Whitelist sort columns
            var orderColumn = (sortBy ?? "").ToLowerInvariant() switch
            {
                "date" => "charge_date",
                "datetime" => "charge_datetime",
                _ => "charge_id"
            };
            var orderDir = descending ? "DESC" : "ASC";

            var safePage = Math.Max(1, pageNumber);
            var safeSize = Math.Clamp(pageSize, 1, 200);
            var offset = (safePage - 1) * safeSize;

            // Count (filtered)
            const string countSql = @"
                SELECT COUNT(1) AS TotalCount
                FROM mg_charge
                WHERE charge_member_id = @memberId;";
            var countParams = new Dictionary<string, object> { ["@memberId"] = memberId };
            var countTable = await SelectQueryAsync(countSql, countParams);
            var total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // Build ORDER BY safely (avoid duplicate column)
            var orderBySql = orderColumn == "charge_id"
                ? $"ORDER BY charge_id {orderDir}"
                : $"ORDER BY {orderColumn} {orderDir}, charge_id ASC";

            // Page (filtered)
            var pageSql = $@"
                SELECT charge_id, charge_member_id, charge_agent_id,
                    charge_gamemoney, charge_currency, charge_date, charge_datetime
                FROM mg_charge
                WHERE charge_member_id = @memberId
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
                items.Add(row.ToChargeFromDataRow());

            return (items, total);
        }
        public async Task<(List<Charge> items, int total)> GetPaginatedChargesAsync(
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize)
        {
            // Manually map DataTable rows to a List<Game>
            var items = new List<Charge>();

            // Whitelist sortable columns to avoid ORDER BY injection
            string orderColumn = sortBy?.ToLowerInvariant() switch
            {
                "date" => "charge_date",
                "datetime" => "charge_datetime",
                _ => "charge_id"
            };
            string orderDir = descending ? "DESC" : "ASC";

            int offset = Math.Max(0, (pageNumber - 1) * pageSize);

            // Get total count separately using SelectQueryAsync
            const string countSql = @"SELECT COUNT(1) AS TotalCount FROM mg_charge;";
            DataTable countTable = await SelectQueryAsync(countSql);
            int total = countTable.Rows.Count > 0 ? Convert.ToInt32(countTable.Rows[0]["TotalCount"]) : 0;

            // Get paginated data using SelectQueryAsync
            string pageSql = $@"
                SELECT 
                    charge_id, charge_member_id, charge_agent_id, charge_gamemoney, charge_currency, charge_date, charge_datetime
                FROM mg_charge
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
                items.Add(row.ToChargeFromDataRow());
            }

            return (items, total);
        }
    }
}
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IAgentRepository
    {
        Task<Agent?> GetAgentByCodeAsync(string code);
    }

    public class AgentRepository : BaseRepository, IAgentRepository
    {
        public AgentRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<Agent?> GetAgentByCodeAsync(string code)
        {
            const string query = @"
                SELECT
                    agent_id, 
                    agent_code, 
                    agent_name, 
                    agent_language, 
                    agent_currency, 
                    agent_money, 
                    agent_deferred, 
                    agent_percent_type, 
                    agent_status, 
                    agent_wallet, 
                    agent_seamless_url
                FROM mg_agent
                WHERE agent_code = @code;";

            var parameters = new Dictionary<string, object>
            {
                { "@code", code }
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                return row.ToAgentFromDataRow();
            }

            return null;
        }
    }
}
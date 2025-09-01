using AGAMinigameApi.Interfaces;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public class AgentRepository : BaseRepository, IAgentRepository
    {
        public AgentRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<Agent?> GetAgentByCodeAsync(string code)
        {
            const string query = @"SELECT
                    agent_id, agent_code, agent_password, agent_name, agent_uplevel_id,
                    agent_language, agent_currency, agent_money, agent_datetime, agent_update,
                    agent_ip, agent_affiliate, agent_deferred, agent_percent_type, agent_status,
                    agent_multilogin, agent_session, agent_wallet, agent_seamless_url, agent_return_url
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
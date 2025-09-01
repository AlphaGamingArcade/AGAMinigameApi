using AGAMinigameApi.Interfaces;

namespace AGAMinigameApi.Repositories
{
    public class AgentRepository : BaseRepository, IAgentRepository
    {
        public AgentRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<bool> AgentExistsByCodeAsync(string code)
        {
            const string query = "SELECT agent_code FROM mg_agent WHERE agent_code = @code;";
            var parameters = new Dictionary<string, object>
            {
                { "@code", code }
            };

            var dataTable = await SelectQueryAsync(query, parameters);
            return dataTable.Rows.Count > 0;
        }
    }
}
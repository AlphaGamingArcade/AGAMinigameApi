using AGAMinigameApi.Interfaces;
using AGAMinigameApi.Models;

namespace AGAMinigameApi.Services
{
    public interface IAgentService
    {
        Task<Agent?> GetAgentByCodeAsync(string code); 
    }

    public class AgentService : IAgentService
    {
        private readonly IAgentRepository _agentRespository;

        public AgentService(IAgentRepository agentRespository)
        {
            _agentRespository = agentRespository;
        }
        public async Task<Agent?> GetAgentByCodeAsync(string code) => await _agentRespository.GetAgentByCodeAsync(code);
    }
}
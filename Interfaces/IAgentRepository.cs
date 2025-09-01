using AGAMinigameApi.Models;

namespace AGAMinigameApi.Interfaces
{
    public interface IAgentRepository
    {
        Task<bool> AgentExistsByCodeAsync(string code);
    }
}
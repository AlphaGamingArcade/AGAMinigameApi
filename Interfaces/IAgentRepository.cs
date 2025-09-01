using AGAMinigameApi.Dtos.Agent;
using AGAMinigameApi.Models;

namespace AGAMinigameApi.Interfaces
{
    public interface IAgentRepository
    {
        Task<Agent?> GetAgentByCodeAsync(string code);
    }
}
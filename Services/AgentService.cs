using AGAMinigameApi.Interfaces;

namespace AGAMinigameApi.Services
{
    public interface IAgentService
    {
        Task<bool> AgentExistsByCodeAsync(string email);
    }

    public class AgentService : IAgentService
    {
        private readonly IAuthRepository _authRepository;

        public AgentService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<bool> AgentExistsByCodeAsync(string email) => await _authRepository.UserExistsByEmailAsync(email);
    }
}
using AGAMinigameApi.Dtos.Agent;
using AGAMinigameApi.Models;

namespace AGAMinigameApi.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
        Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken);
        Task<int> DeleteRefreshTokenByMemberIdAsync(int memberId);
    }
}
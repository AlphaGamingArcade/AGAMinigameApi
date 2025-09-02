using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;

namespace AGAMinigameApi.Services
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token); 
    }

    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string refreshToken) => await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
    }
}
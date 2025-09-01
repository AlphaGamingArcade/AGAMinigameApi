using AGAMinigameApi.Dtos.Auth;
using AGAMinigameApi.Models; // Assuming you have an ApplicationUser model

namespace AGAMinigameApi.Services
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}


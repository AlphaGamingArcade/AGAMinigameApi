using AGAMinigameApi.Models;

namespace AGAMinigameApi.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> UserExistsAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task UpdatePasswordAsync(int userId, string newPassword);
    }
}
using AGAMinigameApi.Models;

namespace AGAMinigameApi.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> UserExistsByEmailAsync(string email);
        Task<bool> UserExistsByAccountAsync(string account);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user, DateTime dateTime);
        Task UpdatePasswordAsync(int userId, string newPassword);
    }
}
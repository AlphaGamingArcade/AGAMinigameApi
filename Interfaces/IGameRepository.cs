using AGAMinigameApi.Models;

namespace AGAMinigameApi.Interfaces
{
    public interface IGameRepository
    {
        Task<(List<Game> items, int total)> GetPaginatedGamesAsync(
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize
        );
        // Task<Banner> GetById(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }
}
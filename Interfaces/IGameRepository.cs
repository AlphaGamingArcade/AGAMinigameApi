using AGAMinigameApi.Models;

namespace AGAMinigameApi.Interfaces
{
    public interface IGameRepository
    {
        Task<IEnumerable<Game>> GetAll();
        // Task<Banner> GetById(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }
}
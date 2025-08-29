using AGAMinigameApi.Models;

namespace AGAMinigameApi.Interfaces
{
    public interface IBannerRepository
    {
        Task<IEnumerable<Banner>> GetAll();
        // Task<Banner> GetById(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }
}
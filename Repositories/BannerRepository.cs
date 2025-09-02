using System.Data;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public interface IBannerRepository
    {
        Task<IEnumerable<Banner>> GetAll();
        // Task<Banner> GetById(int id);
        // Task<int> Add(Banner banner);
        // Task<int> Update(Banner banner);
        // Task<int> Delete(int id);
    }

    public class BannerRepository : BaseRepository, IBannerRepository
    {
        public BannerRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<IEnumerable<Banner>> GetAll()
        {
            var banners = new List<Banner>();
            var table = await SelectQueryAsync("SELECT * FROM mg_banner");
            foreach (DataRow row in table.Rows)
            {
                banners.Add(row.ToBannerFromDataRow());
            }
            return banners;
        }
    }
}
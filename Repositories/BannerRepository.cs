using System.Data;
using AGAMinigameApi.Interfaces;
using AGAMinigameApi.Models;
using api.Mappers;
using Microsoft.Data.SqlClient;

namespace AGAMinigameApi.Repositories
{
    public class BannerRepository : BaseRepository, IBannerRepository
    {
        public BannerRepository(IConfiguration configuration) : base(configuration){}

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
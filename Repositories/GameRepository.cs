using System.Data;
using AGAMinigameApi.Interfaces;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public class GameRepository : BaseRepository, IGameRepository
    {
        public GameRepository(IConfiguration configuration) : base (configuration){}

        public async Task<IEnumerable<Game>> GetAll()
        {
            var games = new List<Game>();
            var table = await SelectQueryAsync("SELECT * FROM mg_game");
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    games.Add(row.ToGameFromDataRow());
                }
                return games;
            }
            catch (Exception)
            {   
                throw;
            }
        }
    }
}
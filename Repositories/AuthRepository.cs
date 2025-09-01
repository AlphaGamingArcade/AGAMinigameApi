using System.Data;
using AGAMinigameApi.Interfaces;
using AGAMinigameApi.Models;
using api.Mappers;

namespace AGAMinigameApi.Repositories
{
    public class AuthRepository : BaseRepository, IAuthRepository
    {
        public AuthRepository(IConfiguration configuration) : base(configuration) { }
        
        
    }
}
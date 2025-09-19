using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Dtos.Favorite;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;
using api.Mappers;

namespace AGAMinigameApi.Services
{
    public interface IFavoriteService
    {
        Task<GameDto?> GetMemberFavoriteAsync(int memberId, int gameId);
        Task<bool> IsMemberFavoriteExistsAsync(int memberId, int gameId);
        Task<FavoriteDto> CreateMemberFavoriteAsync(int memberId, CreateFavoriteDto createDto);
        Task<PagedResult<GameDto>> GetPaginatedMemberFavoritesAsync(int memberId, PagedRequestDto requestDto);
    }

    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<bool> IsMemberFavoriteExistsAsync(int memberId, int gameId) => await _favoriteRepository.ExistsAsync(memberId, gameId);

        public async Task<GameDto?> GetMemberFavoriteAsync(int memberId, int gameId)
        {
            var game = await _favoriteRepository.GetFavoriteByMemberIdAndGameIdAsync(memberId, gameId);
            if (game is null) return null;
            return game.ToGameDto();
        }
        
        public async Task<FavoriteDto> CreateMemberFavoriteAsync(int memberId, CreateFavoriteDto createDto)
        {
            var now = DateHelper.GetUtcNow();
            var favorite = new Favorite
            {
                MemberId = memberId,
                GameId = createDto.GameId,
                CreatedAt = now,
                UpdatedAt = null
            };

            var result = await _favoriteRepository.AddAsync(favorite);
            return result.ToFavoriteDto();
        }

        public async Task<PagedResult<GameDto>> GetPaginatedMemberFavoritesAsync(int memberId, PagedRequestDto requestDto)
        {
            var (games, total) = await _favoriteRepository.GetPaginatedFavoritesByMemberIdAsync(
                memberId,
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize
            );

            var gameDtos = games.Select(g => g.ToGameDto());

            var pagedResult = new PagedResult<GameDto>
            {
                Items = gameDtos,
                TotalRecords = total,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize
            };

            return pagedResult;
        }
    }
}
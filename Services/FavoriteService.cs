using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Dtos.Favorite;
using AGAMinigameApi.Repositories;
using api.Mappers;

namespace AGAMinigameApi.Services
{
    public interface IFavoriteService
    {
        Task<PagedResult<FavoriteDto>> GetPaginatedMemberFavoritesAsync(int memberId, PagedRequestDto requestDto);
    }

    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<PagedResult<FavoriteDto>> GetPaginatedMemberFavoritesAsync(int memberId, PagedRequestDto requestDto)
        {
            var (favorites, total) = await _favoriteRepository.GetPaginatedFavoritesByMemberIdAsync(
                memberId,
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize
            );

            var favoriteDtos = favorites.Select(g => g.ToFavoriteDto());

            var pagedResult = new PagedResult<FavoriteDto>
            {
                Items = favoriteDtos,
                TotalRecords = total,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize
            };

            return pagedResult;
        }
    }
}
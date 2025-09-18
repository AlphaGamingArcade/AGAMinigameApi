using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Repositories;
using api.Mappers;

namespace AGAMinigameApi.Services
{
    public interface IGameService
    {
        Task<PagedResult<GameDto>> GetPaginatedMemberGamesAsync(int memberId, PagedRequestDto requestDto);
        Task<PagedResult<GameDto>> GetPaginatedGamesAsync(PagedRequestDto requestDto);
    }

    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<PagedResult<GameDto>> GetPaginatedMemberGamesAsync(int memberId, PagedRequestDto requestDto)
        {
            var (games, total) = await _gameRepository.GetPaginatedGamesAsync(
                requestDto.Search,
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize
            );

            var gameDtos = games.Select(g =>
            {
                var dto = g.ToGameDto();
                dto.Url = $"{dto.Url}?{memberId}";
                return dto;
            });

            var pagedResult = new PagedResult<GameDto>
            {
                Items = gameDtos,
                TotalRecords = total,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize
            };

            return pagedResult;
        }
        

        public async Task<PagedResult<GameDto>> GetPaginatedGamesAsync(PagedRequestDto requestDto)
        {
            var (games, total) = await _gameRepository.GetPaginatedGamesAsync(
                requestDto.Search,
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
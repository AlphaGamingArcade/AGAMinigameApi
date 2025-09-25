using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Repositories;
using api.Mappers;

namespace AGAMinigameApi.Services
{
    public interface IGameService
    {
        Task<GameDto?> GetGameAsync(int gameId);
        Task<PagedResult<GameDto>> GetPaginatedMemberGamesAsync(int memberId, PagedRequestDto requestDto);
        Task<PagedResult<GameDto>> GetPaginatedGamesAsync(PagedRequestDto requestDto);
        Task<PagedResult<GameDto>> GetTopPaginatedGamesAsync(PagedRequestDto requestDto);
        Task<PagedResult<GameDto>> GetTrendingPaginatedGamesAsync(PagedRequestDto requestDto);
         Task<PagedResult<GameDto>> GetLatestPaginatedGamesAsync(PagedRequestDto requestDto);
    }

    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<GameDto?> GetGameAsync(int gameId)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game is null) return null;
            return game.ToGameDto();
        }

        public async Task<PagedResult<GameDto>> GetPaginatedMemberGamesAsync(int memberId, PagedRequestDto requestDto)
        {
            var (games, total) = await _gameRepository.GetPaginatedGamesAsync(
                requestDto.Search,
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize,
                's'
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


        public async Task<PagedResult<GameDto>> GetPaginatedGamesAsync(PagedRequestDto requestDto)
        {
            var (games, total) = await _gameRepository.GetPaginatedGamesAsync(
                requestDto.Search,
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize,
                's'
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

        public async Task<PagedResult<GameDto>> GetTopPaginatedGamesAsync(PagedRequestDto requestDto)
        {
            var (games, total) = await _gameRepository.GetPaginatedGamesAsync(
                requestDto.Search,
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize,
                's',
                true // top
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

        public async Task<PagedResult<GameDto>> GetTrendingPaginatedGamesAsync(PagedRequestDto requestDto)
        {
            var (games, total) = await _gameRepository.GetPaginatedGamesAsync(
                requestDto.Search,
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize,
                's',
                false,
                true // trending
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

        public async Task<PagedResult<GameDto>> GetLatestPaginatedGamesAsync(PagedRequestDto requestDto)
        {
            var (games, total) = await _gameRepository.GetPaginatedGamesAsync(
                requestDto.Search,
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize,
                's',
                false,
                false,
                true
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
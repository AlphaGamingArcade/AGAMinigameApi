using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Dtos.Game;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;
using api.Mappers;

namespace AGAMinigameApi.Services
{
    public interface IPlayService
    {
        Task<PagedResult<GameDto>> GetPaginatedMemberPlaysAsync(int memberId, PagedRequestDto requestDto); 
    }

    public class PlayService : IPlayService
    {
        private readonly IPlayRepository _playRepository;

        public PlayService(IPlayRepository playRepository)
        {
            _playRepository = playRepository;
        }

        public async Task<PagedResult<GameDto>> GetPaginatedMemberPlaysAsync(int memberId, PagedRequestDto requestDto)
        {
            var (games, total) = await _playRepository.GetPaginatedPlaysByMemberIdAsync(
                memberId,
                requestDto.Search,
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize,
                's');

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
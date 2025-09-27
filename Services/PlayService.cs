using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Dtos.Game;
using AGAMinigameApi.Dtos.Play;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;
using api.Mappers;

namespace AGAMinigameApi.Services
{
    public interface IPlayService
    {
        Task<PlayDto?> GetMemberPlayAsync(int memberId, int gameId);
        Task<bool> IsMemberPlayExistsAsync(int memberId, int gameId);
        Task<PlayDto> UpdateMemberPlayAsync(int memberId, PlayDto playDto);
        Task<PlayDto> CreateMemberPlayAsync(int memberId, CreatePlayRequestDto createDto);
        Task<PagedResult<GameDto>> GetPaginatedMemberPlaysAsync(int memberId, PagedRequestDto requestDto);
    }

    public class PlayService : IPlayService
    {
        private readonly IPlayRepository _playRepository;

        public PlayService(IPlayRepository playRepository)
        {
            _playRepository = playRepository;
        }

        public async Task<bool> IsMemberPlayExistsAsync(int memberId, int gameId) => await _playRepository.ExistsAsync(memberId, gameId);

        public async Task<PlayDto?> GetMemberPlayAsync(int memberId, int gameId)
        {
            var play = await _playRepository.GetPlayByMemberIdAndGameIdAsync(memberId, gameId);
            if (play is null) return null;
            return play.ToPlayDto();
        }

        public async Task<PlayDto> CreateMemberPlayAsync(int memberId, CreatePlayRequestDto createDto)
        {
            var now = DateHelper.GetUtcNow();
            var play = new Play
            {
                MemberId = memberId,
                GameId = createDto.GameId,
                CreatedAt = now,
                UpdatedAt = now
            };

            var result = await _playRepository.AddAsync(play);
            return result.ToPlayDto();
        }

        public async Task<PlayDto> UpdateMemberPlayAsync(int memberId, PlayDto playDto)
        {
            var now = DateHelper.GetUtcNow();
            var play = new Play
            {
                Id = playDto.Id,
                MemberId = memberId,
                GameId = playDto.GameId,
                CreatedAt = playDto.CreatedAt,
                UpdatedAt = now // One needs update
            };
            var result = await _playRepository.UpdateAsync(play);
            return result.ToPlayDto();
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
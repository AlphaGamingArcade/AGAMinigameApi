using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Repositories;
using api.Mappers;

namespace AGAMinigameApi.Services
{
    public interface IBettingService
    {
        Task<PagedResult<BettingDto>> GetPaginatedMemberBettingsAsync(int memberId, PagedRequestDto requestDto);
    }

    public class BettingService : IBettingService
    {
        private readonly IBettingRepository _bettingRespository;

        public BettingService(IBettingRepository bettingRepository)
        {
            _bettingRespository = bettingRepository;
        }

        public async Task<PagedResult<BettingDto>> GetPaginatedMemberBettingsAsync(int memberId, PagedRequestDto requestDto)
        {
            var (bettings, total) = await _bettingRespository.GetPaginatedBettingsByMemberIdAsync(
                memberId,
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize
            );

            var bettingDtos = bettings.Select(g => g.ToBettingDto());

            var pagedResult = new PagedResult<BettingDto>
            {
                Items = bettingDtos,
                TotalRecords = total,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize
            };

            return pagedResult;
        }
    }
}
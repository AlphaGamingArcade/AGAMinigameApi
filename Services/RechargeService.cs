using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Repositories;
using api.Mappers;

namespace AGAMinigameApi.Services
{
    public interface IRechargeService
    {
        Task<PagedResult<RechargeDto>> GetPaginatedRechargesAsync(PagedRequestDto requestDto);
    }

    public class RechargeService : IRechargeService
    {
        private readonly IRechargeRepository _rechargeRepository;

        public RechargeService(IRechargeRepository rechargeRepository)
        {
            _rechargeRepository = rechargeRepository;
        }

        public async Task<PagedResult<RechargeDto>> GetPaginatedRechargesAsync(PagedRequestDto requestDto)
        {
            var (recharges, total) = await _rechargeRepository.GetPaginatedRechargesAsync(
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize
            );

            var rechargeDtos = recharges.Select(g => g.ToRechargeDto());

            var pagedResult = new PagedResult<RechargeDto>
            {
                Items = rechargeDtos,
                TotalRecords = total,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize
            };

            return pagedResult;
        }
    }
}
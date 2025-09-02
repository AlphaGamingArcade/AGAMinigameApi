using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Repositories;
using api.Mappers;

namespace AGAMinigameApi.Services
{
    public interface IBannerService
    {
        Task<PagedResult<BannerDto>> GetPaginatedBannersAsync(PagedRequestDto requestDto);
    }

    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _bannerRepository;

        public BannerService(IBannerRepository bannerRepository)
        {
            _bannerRepository = bannerRepository;
        }

        public async Task<PagedResult<BannerDto>> GetPaginatedBannersAsync(PagedRequestDto requestDto)
        {
            var (banners, total) = await _bannerRepository.GetPaginatedBannersAsync(
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize
            );

            var bannetDtos = banners.Select(g => g.ToBannerDto());

            var pagedResult = new PagedResult<BannerDto>
            {
                Items = bannetDtos,
                TotalRecords = total,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize
            };

            return pagedResult;
        }
    }
}
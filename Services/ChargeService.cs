using AGAMinigameApi.Dtos.Banner;
using AGAMinigameApi.Dtos.Charge;
using AGAMinigameApi.Dtos.Common;
using AGAMinigameApi.Dtos.Member;
using AGAMinigameApi.Models;
using AGAMinigameApi.Repositories;
using api.Mappers;

namespace AGAMinigameApi.Services
{
    public interface IChargeService
    {
        Task<bool> IsChargeExistsAsync(int memberId, DateTime dateTime);
        Task<ChargeDto> ChargeMemberAsync(MemberDto member, DateTime dateTime, decimal amount);
        Task<PagedResult<ChargeDto>> GetPaginatedChargesAsync(PagedRequestDto requestDto);
        Task<PagedResult<ChargeDto>> GetPaginatedMemberChargesAsync(int memberId, PagedRequestDto requestDto);
    }

    public class ChargeService : IChargeService
    {
        private readonly IChargeRepository _chargeRepository;
        private readonly IMemberRepository _memberRepository;
        public ChargeService(IChargeRepository chargeRepository, IMemberRepository memberRepository)
        {
            _chargeRepository = chargeRepository;
            _memberRepository = memberRepository;
        }

        public async Task<bool> IsChargeExistsAsync(int memberId, DateTime dateTime)
        {
            return await _chargeRepository.IsChargeExistsAsync(memberId, dateTime); ;
        }
        
        public async Task<ChargeDto> ChargeMemberAsync(MemberDto member, DateTime dateTime, decimal amount)
        {
            var charge = new Charge
            {
                MemberId = member.Id,
                AgentId = member.AgentId,
                Gamemoney = amount,
                Currency = member.Currency ?? "",
                Date = DateOnly.FromDateTime(dateTime),
                Datetime = dateTime
            };
            var newCharge = await _chargeRepository.AddAsync(charge);

            await _memberRepository.UpdateOnChargeAsync(member.Id, amount);

            return newCharge.ToChargeDto();
        }

        public async Task<PagedResult<ChargeDto>> GetPaginatedChargesAsync(PagedRequestDto requestDto)
        {
            var (charges, total) = await _chargeRepository.GetPaginatedChargesAsync(
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize
            );

            var ChargeDtos = charges.Select(g => g.ToChargeDto());

            var pagedResult = new PagedResult<ChargeDto>
            {
                Items = ChargeDtos,
                TotalRecords = total,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize
            };

            return pagedResult;
        }

        public async Task<PagedResult<ChargeDto>> GetPaginatedMemberChargesAsync(int memberId, PagedRequestDto requestDto)
        {
            var (charges, total) = await _chargeRepository.GetPaginatedChargesByMemberIdAsync(
                memberId,
                requestDto.SortBy,
                requestDto.Descending,
                requestDto.PageNumber,
                requestDto.PageSize
            );

            var ChargeDtos = charges.Select(g => g.ToChargeDto());

            var pagedResult = new PagedResult<ChargeDto>
            {
                Items = ChargeDtos,
                TotalRecords = total,
                PageNumber = requestDto.PageNumber,
                PageSize = requestDto.PageSize
            };

            return pagedResult;
        }
    }
}
using AGAMinigameApi.Dtos.Member;
using AGAMinigameApi.Helpers;
using AGAMinigameApi.Repositories;
using api.Mappers;

namespace AGAMinigameApi.Services
{
    public interface IMemberService
    {
        Task UpdateMemberNicknameAsync(int memberId, string newNickname);
        Task<MemberDto?> GetMemberByIdAsync(int memberId);
    }

    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<MemberDto?> GetMemberByIdAsync(int memberId)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null) return null;
            return member.ToMemberDto();
        }
        
        public async Task UpdateMemberNicknameAsync(int memberId, string newNickname)
        {
            var now = DateHelper.GetUtcNow();
            await _memberRepository.PatchNicknameAsync(memberId, newNickname, now);
        }
    }
}
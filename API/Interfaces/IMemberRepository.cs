using API.Entities;

namespace API.Interfaces
{
    public interface IMemberRepository
    {
        void Update(Member member);
        Task<Boolean> SaveAllAsync();
        Task<IReadOnlyList<Member>> GetMembersAsync();
        Task<Member?> GetMemberByIdAsync(string id);
        Task<IReadOnlyList<Photo>> GetMemberPhotosAsync(string memberId);
    }
}
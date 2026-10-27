using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private IMemberRepository? _memberRepository;
        private IMessageRepository? messageRepository;
        private ILikesRepository? likesRepository;

        public IMemberRepository MemberRepository => _memberRepository ??= new MemberRepository(context);
        public IMessageRepository MessageRepository => messageRepository ??= new MessageRepository(context);

        public ILikesRepository LikesRepository => likesRepository ??= new LikesRepository(context);

        public async Task<bool> Complete()
        {
            try
            {
                return await context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurd while saving changes", ex);
            }
        }

        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }
    }
}
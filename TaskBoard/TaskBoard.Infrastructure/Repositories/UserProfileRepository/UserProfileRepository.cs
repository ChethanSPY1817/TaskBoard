using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories.UserProfileRepository;

namespace TaskBoard.Infrastructure.Repositories.UserProfileRepository
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext _context;
        public UserProfileRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<UserProfile>> GetAllAsync() =>
            await _context.UserProfiles.Include(p => p.User).AsNoTracking().ToListAsync();

        public async Task<UserProfile?> GetByIdAsync(Guid userId) =>
            await _context.UserProfiles.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);

        public async Task AddAsync(UserProfile profile)
        {
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserProfile profile)
        {
            _context.UserProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserProfile profile)
        {
            _context.UserProfiles.Remove(profile);
            await _context.SaveChangesAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories.UserRepository;

namespace TaskBoard.Infrastructure.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<User>> GetAllAsync() =>
            await _context.Users.Include(u => u.Role).AsNoTracking().ToListAsync();

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}

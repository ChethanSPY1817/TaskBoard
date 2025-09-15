using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories.RoleRepository;

namespace TaskBoard.Infrastructure.Repositories.RoleRepository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Role>> GetAllAsync() =>
            await _context.Roles.AsNoTracking().ToListAsync();

        public async Task<Role?> GetByIdAsync(Guid id) =>
            await _context.Roles.FindAsync(id);

        public async Task AddAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Role role)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}

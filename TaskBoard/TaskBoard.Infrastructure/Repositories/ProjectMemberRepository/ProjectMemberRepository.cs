using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories.ProjectMemberRepository;

namespace TaskBoard.Infrastructure.Repositories.ProjectMemberRepository
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectMemberRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<ProjectMember>> GetAllAsync() =>
            await _context.ProjectMembers.Include(pm => pm.User)
                                         .Include(pm => pm.Project)
                                         .AsNoTracking()
                                         .ToListAsync();

        public async Task<ProjectMember?> GetByIdAsync(Guid projectId, Guid userId) =>
            await _context.ProjectMembers
                          .Include(pm => pm.User)
                          .Include(pm => pm.Project)
                          .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

        public async Task AddAsync(ProjectMember member)
        {
            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectMember member)
        {
            _context.ProjectMembers.Update(member);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProjectMember member)
        {
            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();
        }
    }
}

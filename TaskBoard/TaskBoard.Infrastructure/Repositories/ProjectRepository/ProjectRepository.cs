using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories.ProjectRepository;

namespace TaskBoard.Infrastructure.Repositories.P
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Project>> GetAllAsync() =>
            await _context.Projects.Include(p => p.Owner).Include(p => p.Members).AsNoTracking().ToListAsync();

        public async Task<Project?> GetByIdAsync(Guid id) =>
            await _context.Projects.Include(p => p.Owner).Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Project project)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}

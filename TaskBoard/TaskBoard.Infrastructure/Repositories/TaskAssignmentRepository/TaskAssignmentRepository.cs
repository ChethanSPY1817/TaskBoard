using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories.TaskAssignmentRepository;

namespace TaskBoard.Infrastructure.Repositories.TaskAssignmentRepository
{
    public class TaskAssignmentRepository : ITaskAssignmentRepository
    {
        private readonly ApplicationDbContext _context;
        public TaskAssignmentRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<TaskAssignment>> GetAllAsync() =>
            await _context.TaskAssignments
                          .Include(a => a.TaskItem)
                          .Include(a => a.AssignedToUser)
                          .Include(a => a.AssignedByUser)
                          .AsNoTracking()
                          .ToListAsync();

        public async Task<TaskAssignment?> GetByIdAsync(Guid id) =>
            await _context.TaskAssignments
                          .Include(a => a.TaskItem)
                          .Include(a => a.AssignedToUser)
                          .Include(a => a.AssignedByUser)
                          .FirstOrDefaultAsync(a => a.Id == id);

        public async Task AddAsync(TaskAssignment assignment)
        {
            _context.TaskAssignments.Add(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskAssignment assignment)
        {
            _context.TaskAssignments.Update(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskAssignment assignment)
        {
            _context.TaskAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }
}

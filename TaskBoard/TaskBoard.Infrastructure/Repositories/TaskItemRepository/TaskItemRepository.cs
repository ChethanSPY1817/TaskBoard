using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories.TaskItemRepository;

namespace TaskBoard.Infrastructure.Repositories.TaskItemRepository
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly ApplicationDbContext _context;
        public TaskItemRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<TaskItem>> GetAllAsync() =>
            await _context.Tasks.AsNoTracking().ToListAsync();

        public async Task<TaskItem?> GetByIdAsync(Guid id) =>
            await _context.Tasks.FindAsync(id);

        public async Task AddAsync(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}

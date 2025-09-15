using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories.TaskTagRepository;

namespace TaskBoard.Infrastructure.Repositories.TaskTagRepository
{
    public class TaskTagRepository : ITaskTagRepository
    {
        private readonly ApplicationDbContext _context;
        public TaskTagRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<TaskTag>> GetAllAsync() =>
            await _context.TaskTags.Include(tt => tt.TaskItem)
                                   .Include(tt => tt.Tag)
                                   .AsNoTracking()
                                   .ToListAsync();

        public async Task<TaskTag?> GetByIdAsync(Guid taskId, Guid tagId) =>
            await _context.TaskTags.Include(tt => tt.TaskItem)
                                   .Include(tt => tt.Tag)
                                   .FirstOrDefaultAsync(tt => tt.TaskItemId == taskId && tt.TagId == tagId);

        public async Task AddAsync(TaskTag taskTag)
        {
            _context.TaskTags.Add(taskTag);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskTag taskTag)
        {
            _context.TaskTags.Update(taskTag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskTag taskTag)
        {
            _context.TaskTags.Remove(taskTag);
            await _context.SaveChangesAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories.TagRepository;

namespace TaskBoard.Infrastructure.Repositories.TagRepository
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;
        public TagRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Tag>> GetAllAsync() =>
            await _context.Tags.Include(t => t.TaskTags)
                               .AsNoTracking()
                               .ToListAsync();

        public async Task<Tag?> GetByIdAsync(Guid id) =>
            await _context.Tags.Include(t => t.TaskTags)
                               .FirstOrDefaultAsync(t => t.Id == id);

        public async Task AddAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Tag tag)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }
}

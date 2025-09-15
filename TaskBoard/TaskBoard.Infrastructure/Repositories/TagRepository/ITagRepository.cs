using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Repositories.TagRepository
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(Guid id);
        Task AddAsync(Tag tag);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(Tag tag);
    }
}

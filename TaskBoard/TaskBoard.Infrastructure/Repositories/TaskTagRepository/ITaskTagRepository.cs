using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Repositories.TaskTagRepository
{
    public interface ITaskTagRepository
    {
        Task<List<TaskTag>> GetAllAsync();
        Task<TaskTag?> GetByIdAsync(Guid taskId, Guid tagId);
        Task AddAsync(TaskTag taskTag);
        Task UpdateAsync(TaskTag taskTag);
        Task DeleteAsync(TaskTag taskTag);
    }
}

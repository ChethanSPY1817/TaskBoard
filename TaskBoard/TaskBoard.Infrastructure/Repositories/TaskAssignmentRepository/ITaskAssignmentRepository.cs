using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Repositories.TaskAssignmentRepository
{
    public interface ITaskAssignmentRepository
    {
        Task<List<TaskAssignment>> GetAllAsync();
        Task<TaskAssignment?> GetByIdAsync(Guid id);
        Task AddAsync(TaskAssignment assignment);
        Task UpdateAsync(TaskAssignment assignment);
        Task DeleteAsync(TaskAssignment assignment);
    }
}

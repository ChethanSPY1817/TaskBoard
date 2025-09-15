using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Repositories.ProjectMemberRepository
{
    public interface IProjectMemberRepository
    {
        Task<List<ProjectMember>> GetAllAsync();
        Task<ProjectMember?> GetByIdAsync(Guid projectId, Guid userId);
        Task AddAsync(ProjectMember member);
        Task UpdateAsync(ProjectMember member);
        Task DeleteAsync(ProjectMember member);
    }
}

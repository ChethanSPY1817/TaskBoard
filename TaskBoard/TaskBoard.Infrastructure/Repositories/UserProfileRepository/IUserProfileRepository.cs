using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Repositories.UserProfileRepository
{
    public interface IUserProfileRepository
    {
        Task<List<UserProfile>> GetAllAsync();
        Task<UserProfile?> GetByIdAsync(Guid userId);
        Task AddAsync(UserProfile profile);
        Task UpdateAsync(UserProfile profile);
        Task DeleteAsync(UserProfile profile);
    }
}

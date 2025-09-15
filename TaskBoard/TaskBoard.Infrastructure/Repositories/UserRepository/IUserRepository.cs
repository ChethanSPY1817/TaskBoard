using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user, string plainPassword);
        Task UpdateAsync(User user, string? newPlainPassword = null);
        Task DeleteAsync(User user);
    }
}

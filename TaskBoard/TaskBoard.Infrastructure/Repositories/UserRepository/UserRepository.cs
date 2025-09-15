using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.Infrastructure.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync() =>
            await _context.Users.Include(u => u.Role).AsNoTracking().ToListAsync();

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

        public async Task AddAsync(User user, string plainPassword)
        {
            // Hash the password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // Add user
            _context.Users.Add(user);

            // Store plain-text password in separate table
            var plainPasswordEntry = new PlainTextPassword
            {
                UserId = user.Id,
                Password = plainPassword
            };
            _context.PlainTextPasswords.Add(plainPasswordEntry);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user, string? newPlainPassword = null)
        {
            // If new password provided, hash it and store plain-text
            if (!string.IsNullOrEmpty(newPlainPassword))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPlainPassword);

                var plainPasswordEntry = new PlainTextPassword
                {
                    UserId = user.Id,
                    Password = newPlainPassword
                };
                _context.PlainTextPasswords.Add(plainPasswordEntry);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}

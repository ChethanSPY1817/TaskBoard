using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context) => _context = context;

        // GET: api/Users
        [HttpGet]
        //[Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    RoleId = u.RoleId
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        //[Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    RoleId = u.RoleId
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
        {
            // Check role exists
            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null) return BadRequest("Invalid Role");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = dto.Password, // TODO: hash using Identity or custom hashing
                RoleId = dto.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                RoleId = user.RoleId
            });
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<UserDto>> UpdateUser(Guid id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Update email
            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = dto.Password; // later replace with hashed password

            // Update role if provided
            if (dto.RoleId.HasValue)
            {
                // SuperAdmin role cannot be assigned
                if (dto.RoleId.Value == Guid.Parse("10000000-0000-0000-0000-000000000000"))
                    return BadRequest("Cannot assign SuperAdmin role");

                var role = await _context.Roles.FindAsync(dto.RoleId.Value);
                if (role == null) return BadRequest("Invalid Role");

                user.RoleId = dto.RoleId.Value;
            }

            await _context.SaveChangesAsync();

            // Return updated user
            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                RoleId = user.RoleId
            });
        }


        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Prevent deleting SuperAdmin
            if (user.RoleId == Guid.Parse("10000000-0000-0000-0000-000000000000"))
                return BadRequest("Cannot delete SuperAdmin");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

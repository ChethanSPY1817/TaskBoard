using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;
using BCrypt.Net;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Users
        // Accessible by all authenticated users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .ToListAsync();

            var usersDto = _mapper.Map<List<UserDto>>(users);
            return Ok(usersDto);
        }

        // GET: api/Users/{id}
        // Accessible by all authenticated users
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        // POST: api/Users
        // Only Admin or SuperAdmin
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null) return BadRequest("Invalid Role");

            var user = _mapper.Map<User>(dto);
            user.Id = Guid.NewGuid();

            // Hash the password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _context.Users.Add(user);

            // Store plain-text password in separate table (unsafe for learning/demo only)
            var plainPasswordEntry = new PlainTextPassword
            {
                UserId = user.Id,
                Password = dto.Password
            };
            _context.Set<PlainTextPassword>().Add(plainPasswordEntry);

            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }

        // PUT: api/Users/{id}
        // Only Admin or SuperAdmin
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _mapper.Map(dto, user);

            if (dto.RoleId.HasValue)
            {
                if (dto.RoleId.Value == Guid.Parse("10000000-0000-0000-0000-000000000000"))
                    return BadRequest("Cannot assign SuperAdmin role");

                var role = await _context.Roles.FindAsync(dto.RoleId.Value);
                if (role == null) return BadRequest("Invalid Role");

                user.RoleId = dto.RoleId.Value;
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                var plainPasswordEntry = new PlainTextPassword
                {
                    UserId = user.Id,
                    Password = dto.Password
                };
                _context.Set<PlainTextPassword>().Add(plainPasswordEntry);
            }

            await _context.SaveChangesAsync();

            var updatedUserDto = _mapper.Map<UserDto>(user);
            return Ok(updatedUserDto);
        }

        // DELETE: api/Users/{id}
        // Only Admin or SuperAdmin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.RoleId == Guid.Parse("10000000-0000-0000-0000-000000000000"))
                return BadRequest("Cannot delete SuperAdmin");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Users/{id}
        // Only Admin or SuperAdmin
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> PatchUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            if (dto == null) return BadRequest();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Update only non-null / non-empty fields
            if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // Store plain-text password (for learning/demo only)
                var plainPasswordEntry = new PlainTextPassword
                {
                    UserId = user.Id,
                    Password = dto.Password
                };
                _context.Set<PlainTextPassword>().Add(plainPasswordEntry);
            }

            if (dto.RoleId.HasValue)
            {
                if (dto.RoleId.Value == Guid.Parse("10000000-0000-0000-0000-000000000000"))
                    return BadRequest("Cannot assign SuperAdmin role");

                var role = await _context.Roles.FindAsync(dto.RoleId.Value);
                if (role == null) return BadRequest("Invalid Role");

                user.RoleId = dto.RoleId.Value;
            }

            await _context.SaveChangesAsync();

            var updatedUserDto = _mapper.Map<UserDto>(user);
            return Ok(updatedUserDto);
        }


    }
}
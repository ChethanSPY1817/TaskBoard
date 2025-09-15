using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using TaskBoard.Application.DTOs.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public UsersController(ApplicationDbContext context) => _context = context;

    // GET: api/Users
    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new UserDto { Id = u.Id, Email = u.Email, RoleId = u.RoleId })
            .ToListAsync();

        return Ok(users);
    }

    // GET: api/Users/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDto { Id = u.Id, Email = u.Email, RoleId = u.RoleId })
            .FirstOrDefaultAsync();

        if (user == null) return NotFound();
        return Ok(user);
    }

    // POST: api/Users
    [HttpPost]
    [Authorize(Roles = "SuperAdmin")] // Only SuperAdmin can create users
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
    {
        // Prevent creating another SuperAdmin
        var role = await _context.Roles.FindAsync(dto.RoleId);
        if (role == null || role.Name == "SuperAdmin")
            return BadRequest("Invalid role.");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // hash password
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
    [Authorize(Roles = "SuperAdmin")] // Only SuperAdmin can update users including roles
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        // Prevent updating SuperAdmin role accidentally
        if (dto.RoleId.HasValue)
        {
            var role = await _context.Roles.FindAsync(dto.RoleId.Value);
            if (role == null || role.Name == "SuperAdmin")
                return BadRequest("Invalid role.");
            user.RoleId = dto.RoleId.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email;

        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Users/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")] // Only SuperAdmin can delete users
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        // Prevent deleting SuperAdmin
        var role = await _context.Roles.FindAsync(user.RoleId);
        if (role?.Name == "SuperAdmin")
            return BadRequest("Cannot delete SuperAdmin.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

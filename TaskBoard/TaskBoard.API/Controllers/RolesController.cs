using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.Roles;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public RolesController(ApplicationDbContext context) => _context = context;

    // GET: api/Roles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .Select(r => new RoleDto { Id = r.Id, Name = r.Name })
            .ToListAsync();

        return Ok(roles);
    }

    // GET: api/Roles/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetRole(Guid id)
    {
        var role = await _context.Roles
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RoleDto { Id = r.Id, Name = r.Name })
            .FirstOrDefaultAsync();

        if (role == null) return NotFound();
        return Ok(role);
    }

    // POST: api/Roles
    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleDto dto)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = dto.Name
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, new RoleDto
        {
            Id = role.Id,
            Name = role.Name
        });
    }

    // PUT: api/Roles/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(Guid id, UpdateRoleDto dto)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return NotFound();

        role.Name = dto.Name;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Roles/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return NotFound();

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

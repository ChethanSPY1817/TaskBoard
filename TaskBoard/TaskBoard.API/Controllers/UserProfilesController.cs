using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.UserProfiles;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserProfilesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public UserProfilesController(ApplicationDbContext context) => _context = context;

    // GET: api/UserProfiles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetProfiles()
    {
        var profiles = await _context.UserProfiles
            .AsNoTracking()
            .Select(p => new UserProfileDto
            {
                UserId = p.UserId,
                FullName = p.FullName,
                Phone = p.Phone,
                Address = p.Address
            }).ToListAsync();

        return Ok(profiles);
    }

    // GET: api/UserProfiles/{userId}
    [HttpGet("{userId}")]
    public async Task<ActionResult<UserProfileDto>> GetProfile(Guid userId)
    {
        var profile = await _context.UserProfiles
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new UserProfileDto
            {
                UserId = p.UserId,
                FullName = p.FullName,
                Phone = p.Phone,
                Address = p.Address
            }).FirstOrDefaultAsync();

        if (profile == null) return NotFound();
        return Ok(profile);
    }

    // POST: api/UserProfiles
    [HttpPost]
    public async Task<ActionResult<UserProfileDto>> CreateProfile(CreateUserProfileDto dto)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists) return BadRequest("User does not exist.");

        var exists = await _context.UserProfiles.AnyAsync(p => p.UserId == dto.UserId);
        if (exists) return BadRequest("Profile already exists for this user.");

        var profile = new UserProfile
        {
            UserId = dto.UserId,
            FullName = dto.FullName,
            Phone = dto.Phone,
            Address = dto.Address
        };

        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProfile), new { userId = profile.UserId }, new UserProfileDto
        {
            UserId = profile.UserId,
            FullName = profile.FullName,
            Phone = profile.Phone,
            Address = profile.Address
        });
    }

    // PUT: api/UserProfiles/{userId}
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateProfile(Guid userId, UpdateUserProfileDto dto)
    {
        var profile = await _context.UserProfiles.FindAsync(userId);
        if (profile == null) return NotFound();

        profile.FullName = dto.FullName ?? profile.FullName;
        profile.Phone = dto.Phone ?? profile.Phone;
        profile.Address = dto.Address ?? profile.Address;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/UserProfiles/{userId}
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteProfile(Guid userId)
    {
        var profile = await _context.UserProfiles.FindAsync(userId);
        if (profile == null) return NotFound();

        _context.UserProfiles.Remove(profile);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

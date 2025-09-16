using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskBoard.Application.DTOs.UserProfiles;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class UserProfilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserProfilesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private bool IsSuperAdmin() =>
            User.IsInRole("SuperAdmin");

        private Guid GetCurrentUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: api/UserProfiles
        //[HttpGet]
        //public async Task<IActionResult> GetProfiles()
        //{
        //    if (!IsSuperAdmin())
        //        return Forbid(); // Only SuperAdmin can view all profiles

        //    var profiles = await _context.UserProfiles
        //        .AsNoTracking()
        //        .ToListAsync();

        //    var profilesDto = _mapper.Map<List<UserProfileDto>>(profiles);
        //    return Ok(profilesDto);
        //}

        // GET: api/UserProfiles
        [HttpGet]
        public async Task<IActionResult> GetProfiles(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = "asc")
        {
            if (!IsSuperAdmin())
                return Forbid(); // Only SuperAdmin can view all profiles

            var query = _context.UserProfiles.AsNoTracking();

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "fullname" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName),
                    "phone" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(p => p.Phone) : query.OrderBy(p => p.Phone),
                    "address" => sortOrder.ToLower() == "desc" ? query.OrderByDescending(p => p.Address) : query.OrderBy(p => p.Address),
                    _ => query.OrderBy(p => p.FullName)
                };
            }
            else
            {
                query = query.OrderBy(p => p.FullName);
            }

            // Pagination
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var profiles = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var profilesDto = _mapper.Map<List<UserProfileDto>>(profiles);

            return Ok(new
            {
                page,
                pageSize,
                totalPages,
                totalItems,
                items = profilesDto
            });
        }


        // GET: api/UserProfiles/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            // Only SuperAdmin or the user themselves
            if (!IsSuperAdmin() && GetCurrentUserId() != userId)
                return Forbid();

            var profile = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null) return NotFound();

            var profileDto = _mapper.Map<UserProfileDto>(profile);
            return Ok(profileDto);
        }

        // POST: api/UserProfiles
        [HttpPost]
        public async Task<IActionResult> CreateProfile(CreateUserProfileDto dto)
        {
            // Only SuperAdmin or the user themselves can create a profile
            if (!IsSuperAdmin() && GetCurrentUserId() != dto.UserId)
                return Forbid();

            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists) return BadRequest("User does not exist.");

            var exists = await _context.UserProfiles.AnyAsync(p => p.UserId == dto.UserId);
            if (exists) return BadRequest("Profile already exists for this user.");

            var profile = _mapper.Map<UserProfile>(dto);

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            var profileDto = _mapper.Map<UserProfileDto>(profile);
            return CreatedAtAction(nameof(GetProfile), new { userId = profile.UserId }, profileDto);
        }

        // PUT: api/UserProfiles/{userId}
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateProfile(Guid userId, UpdateUserProfileDto dto)
        {
            // Only SuperAdmin or the user themselves can update
            if (!IsSuperAdmin() && GetCurrentUserId() != userId)
                return Forbid();

            var profile = await _context.UserProfiles.FindAsync(userId);
            if (profile == null) return NotFound();

            _mapper.Map(dto, profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/UserProfiles/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteProfile(Guid userId)
        {
            // Only SuperAdmin can delete profiles
            if (!IsSuperAdmin())
                return Forbid();

            var profile = await _context.UserProfiles.FindAsync(userId);
            if (profile == null) return NotFound();

            _context.UserProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/UserProfiles/{userId}
        // Only SuperAdmin or the user themselves
        [HttpPatch("{userId}")]
        public async Task<IActionResult> PatchProfile(Guid userId, [FromBody] UpdateUserProfileDto dto)
        {
            if (dto == null) return BadRequest("dto is required");

            // Only SuperAdmin or the user themselves can patch
            if (!IsSuperAdmin() && GetCurrentUserId() != userId)
                return Forbid();

            var profile = await _context.UserProfiles.FindAsync(userId);
            if (profile == null) return NotFound();

            // Update only non-null/non-empty properties
            if (!string.IsNullOrEmpty(dto.FullName))
                profile.FullName = dto.FullName;

            if (!string.IsNullOrEmpty(dto.Phone))
                profile.Phone = dto.Phone;

            if (!string.IsNullOrEmpty(dto.Address))
                profile.Address = dto.Address;

            await _context.SaveChangesAsync();

            var updatedDto = _mapper.Map<UserProfileDto>(profile);
            return Ok(updatedDto);
        }

    }
}
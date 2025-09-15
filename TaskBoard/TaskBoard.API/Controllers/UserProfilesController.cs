using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.UserProfiles;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserProfilesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/UserProfiles
        // Returns all user profiles
        [HttpGet]
        public async Task<IActionResult> GetProfiles()
        {
            var profiles = await _context.UserProfiles
                .AsNoTracking()
                .ToListAsync();

            var profilesDto = _mapper.Map<List<UserProfileDto>>(profiles);
            return Ok(profilesDto); // HTTP 200
        }

        // GET: api/UserProfiles/{userId}
        // Returns a single user profile by user ID
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            var profile = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null) return NotFound(); // HTTP 404

            var profileDto = _mapper.Map<UserProfileDto>(profile);
            return Ok(profileDto); // HTTP 200
        }

        // POST: api/UserProfiles
        // Creates a new user profile
        [HttpPost]
        public async Task<IActionResult> CreateProfile(CreateUserProfileDto dto)
        {
            // Validate that the user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists) return BadRequest("User does not exist."); // HTTP 400

            // Prevent duplicate profiles
            var exists = await _context.UserProfiles.AnyAsync(p => p.UserId == dto.UserId);
            if (exists) return BadRequest("Profile already exists for this user."); // HTTP 400

            // Map DTO to entity
            var profile = _mapper.Map<UserProfile>(dto);

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            var profileDto = _mapper.Map<UserProfileDto>(profile);
            return CreatedAtAction(nameof(GetProfile), new { userId = profile.UserId }, profileDto); // HTTP 201
        }

        // PUT: api/UserProfiles/{userId}
        // Updates an existing user profile
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateProfile(Guid userId, UpdateUserProfileDto dto)
        {
            var profile = await _context.UserProfiles.FindAsync(userId);
            if (profile == null) return NotFound(); // HTTP 404

            // Map updates from DTO
            _mapper.Map(dto, profile);

            await _context.SaveChangesAsync();
            return NoContent(); // HTTP 204 indicates success with no content
        }

        // DELETE: api/UserProfiles/{userId}
        // Deletes a user profile
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteProfile(Guid userId)
        {
            var profile = await _context.UserProfiles.FindAsync(userId);
            if (profile == null) return NotFound(); // HTTP 404

            _context.UserProfiles.Remove(profile);
            await _context.SaveChangesAsync();
            return NoContent(); // HTTP 204
        }
    }
}

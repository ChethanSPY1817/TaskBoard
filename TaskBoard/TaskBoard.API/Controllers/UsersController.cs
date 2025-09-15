using AutoMapper;
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
        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Users
        // Retrieves all users from the database and maps them to UserDto
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            var usersDto = _mapper.Map<List<UserDto>>(users);

            // Returns HTTP 200 with the list of users
            return Ok(usersDto);
        }

        // GET: api/Users/{id}
        // Retrieves a single user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(); // Returns HTTP 404 if user not found

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto); // Returns HTTP 200 with user data
        }

        // POST: api/Users
        // Creates a new user in the database
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            // Validate that the role exists
            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null)
                return BadRequest("Invalid Role"); // Returns HTTP 400 if role not found

            // Map DTO to Entity
            var user = _mapper.Map<User>(dto);
            user.Id = Guid.NewGuid(); // Assign a new GUID as ID

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);

            // Returns HTTP 201 with the created user and location header
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
        }

        // PUT: api/Users/{id}
        // Updates an existing user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(); // HTTP 404 if user not found

            // Map changes from DTO to the entity
            _mapper.Map(dto, user);

            // Validate role if provided
            if (dto.RoleId.HasValue)
            {
                // Prevent assigning SuperAdmin role
                if (dto.RoleId.Value == Guid.Parse("10000000-0000-0000-0000-000000000000"))
                    return BadRequest("Cannot assign SuperAdmin role");

                var role = await _context.Roles.FindAsync(dto.RoleId.Value);
                if (role == null)
                    return BadRequest("Invalid Role"); // HTTP 400 if role invalid

                user.RoleId = dto.RoleId.Value;
            }

            await _context.SaveChangesAsync();

            var updatedUserDto = _mapper.Map<UserDto>(user);
            return Ok(updatedUserDto); // HTTP 200 with updated user
        }

        // DELETE: api/Users/{id}
        // Deletes a user from the database
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(); // HTTP 404 if user not found

            // Prevent deleting SuperAdmin
            if (user.RoleId == Guid.Parse("10000000-0000-0000-0000-000000000000"))
                return BadRequest("Cannot delete SuperAdmin"); // HTTP 400

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent(); // HTTP 204 indicates successful deletion
        }
    }
}

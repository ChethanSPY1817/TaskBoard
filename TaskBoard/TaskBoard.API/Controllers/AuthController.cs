using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.Services.Auth;

namespace TaskBoard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenService _jwtService;

        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        // DTOs
        public record RegisterDto(string Email, string Password, string FullName, string? Role = "Developer");
        public record LoginDto(string Email, string Password);

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign role (default Developer)
            var role = dto.Role ?? "Developer";
            if (await _roleManager.RoleExistsAsync(role))
                await _userManager.AddToRoleAsync(user, role);

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid email or password");

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid) return Unauthorized("Invalid email or password");

            var token = await _jwtService.GenerateTokenAsync(user);
            return Ok(new { Token = token });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users.Select(u => new { u.Id, u.Email }).ToList();
            return Ok(users);
        }
    }
}

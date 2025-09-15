using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskBoard.Application.DTOs.Auth;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;
using BCrypt.Net;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // ---------------- Register (SuperAdmin only) ----------------
    [HttpPost("register")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already exists.");

        var role = await _context.Roles.FindAsync(dto.RoleId);
        if (role == null)
            return BadRequest("Invalid RoleId.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            RoleId = dto.RoleId,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password) // <-- BCrypt
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { user.Id, user.Email, Role = role.Name });
    }

    // ---------------- Login ----------------
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users.Include(u => u.Role)
                                       .FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            return Unauthorized("Invalid credentials.");

        // Verify password using BCrypt
        bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!isValid)
            return Unauthorized("Invalid credentials.");

        // Generate JWT
        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }

    // ---------------- JWT Token Generation ----------------
    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role!.Name)
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

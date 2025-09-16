using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskBoard.Application.DTOs.Projects;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ---------------- GET: api/Projects ----------------
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            IQueryable<Project> query = _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Members)
                .AsNoTracking();

            if (userRole == "Developer")
            {
                // Developers see only projects they are part of
                query = query.Where(p => p.Members.Any(m => m.UserId == userId));
            }

            var projects = await query.ToListAsync();
            return Ok(_mapper.Map<List<ProjectDto>>(projects));
        }

        // ---------------- GET: api/Projects/{id} ----------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var project = await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            if (userRole == "Developer" && !project.Members.Any(m => m.UserId == userId))
                return Forbid();

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        // ---------------- POST: api/Projects ----------------
        [HttpPost]
        [Authorize(Roles = "Manager,Admin,SuperAdmin")]
        public async Task<IActionResult> CreateProject(CreateProjectDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // Managers can only create projects with themselves as owner
            if (userRole == "Manager")
                dto.OwnerId = userId;

            // Validate Owner exists
            var ownerExists = await _context.Users.AnyAsync(u => u.Id == dto.OwnerId);
            if (!ownerExists)
                return BadRequest("Owner user does not exist.");

            var project = _mapper.Map<Project>(dto);
            project.Id = Guid.NewGuid();

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, _mapper.Map<ProjectDto>(project));
        }

        // ---------------- PUT: api/Projects/{id} ----------------
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            _mapper.Map(dto, project);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ---------------- DELETE: api/Projects/{id} ----------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Projects/{id}
        // Only Manager, Admin, or SuperAdmin
        [HttpPatch("{id}")]
        [Authorize(Roles = "Manager,Admin,SuperAdmin")]
        public async Task<IActionResult> PatchProject(Guid id, [FromBody] UpdateProjectDto dto)
        {
            if (dto == null) return BadRequest("dto is required");

            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            // Only Manager, Admin, SuperAdmin can patch
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Manager" && project.OwnerId != userId)
                return Forbid(); // Managers can only modify their own projects

            // Patch only non-null / non-empty fields
            if (!string.IsNullOrWhiteSpace(dto.Name))
                project.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                project.Description = dto.Description;

            await _context.SaveChangesAsync();

            var projectDto = _mapper.Map<ProjectDto>(project);
            return Ok(projectDto);
        }

    }
}
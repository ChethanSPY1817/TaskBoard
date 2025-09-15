using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.Projects;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Projects
        // Admin, Manager can see all, Developer will later be restricted
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Developer,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _context.Projects
                .AsNoTracking()
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    OwnerId = p.OwnerId
                })
                .ToListAsync();

            return Ok(projects);
        }

        // GET: api/Projects/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Developer,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
        {
            var project = await _context.Projects
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    OwnerId = p.OwnerId
                })
                .FirstOrDefaultAsync();

            if (project == null) return NotFound();

            return Ok(project);
        }

        // POST: api/Projects
        // Only Admin & Manager can create
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto dto)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                OwnerId = dto.OwnerId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                OwnerId = project.OwnerId
            });
        }

        // PUT: api/Projects/{id}
        // Only Admin & Manager can update
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            project.Name = dto.Name;
            project.Description = dto.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Projects/{id}
        // Only Admin can delete
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

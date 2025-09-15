using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.Projects;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ---------------------- GET: api/Projects ----------------------
        // Returns all projects
        // Accessible by all roles including Developer
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Developer,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Members)
                .AsNoTracking()
                .ToListAsync();

            // Map entity list to DTO list
            return Ok(_mapper.Map<List<ProjectDto>>(projects));
        }

        // ---------------------- GET: api/Projects/{id} ----------------------
        // Returns a specific project by ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Developer,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProject(Guid id)
        {
            var project = await _context.Projects
                .Include(p => p.Owner)
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        // ---------------------- POST: api/Projects ----------------------
        // Creates a new project
        // Only Admin & Manager roles
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProject(CreateProjectDto dto)
        {
            // Map DTO to entity
            var project = _mapper.Map<Project>(dto);
            project.Id = Guid.NewGuid(); // Ensure a new GUID

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, _mapper.Map<ProjectDto>(project));
        }

        // ---------------------- PUT: api/Projects/{id} ----------------------
        // Updates an existing project
        // Only Admin & Manager roles
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            // Map the updated DTO values to the entity
            _mapper.Map(dto, project);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ---------------------- DELETE: api/Projects/{id} ----------------------
        // Deletes a project
        // Only Admin & SuperAdmin roles
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

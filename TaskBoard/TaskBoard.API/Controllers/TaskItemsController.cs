using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.TaskItems;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure;
using TaskBoard.Infrastructure.Data;


namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAll()
        {
            var tasks = await _context.Tasks
                .AsNoTracking()
                .Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    ProjectId = t.ProjectId,
                    AssignedToUserId = t.AssignedToUserId,
                    Status = t.Status,
                    Priority = t.Priority
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDto>> GetById(Guid id)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    ProjectId = t.ProjectId,
                    AssignedToUserId = t.AssignedToUserId,
                    Status = t.Status,
                    Priority = t.Priority
                })
                .FirstOrDefaultAsync();

            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> Create(CreateTaskItemDto dto)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId);
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.AssignedToUserId);

            if (!projectExists) return BadRequest("Project does not exist.");
            if (!userExists) return BadRequest("Assigned user does not exist.");

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                ProjectId = dto.ProjectId,
                AssignedToUserId = dto.AssignedToUserId,
                Status = dto.Status,
                Priority = dto.Priority
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                ProjectId = task.ProjectId,
                AssignedToUserId = task.AssignedToUserId,
                Status = task.Status,
                Priority = task.Priority
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateTaskItemDto dto)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            if (dto.Title != null) task.Title = dto.Title;
            if (dto.Description != null) task.Description = dto.Description;
            if (dto.AssignedToUserId.HasValue) task.AssignedToUserId = dto.AssignedToUserId.Value;
            if (dto.Status.HasValue) task.Status = dto.Status.Value;
            if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

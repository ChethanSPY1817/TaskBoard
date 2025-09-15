using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.TaskTags;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskTagsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public TaskTagsController(ApplicationDbContext context) => _context = context;

    // GET: api/TaskTags
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskTagDto>>> GetTaskTags()
    {
        var taskTags = await _context.TaskTags
            .AsNoTracking()
            .Select(tt => new TaskTagDto
            {
                TaskItemId = tt.TaskItemId,
                TagId = tt.TagId
            })
            .ToListAsync();

        return Ok(taskTags);
    }

    // POST: api/TaskTags
    [HttpPost]
    public async Task<ActionResult<TaskTagDto>> AddTaskTag(CreateTaskTagDto dto)
    {
        var exists = await _context.TaskTags
            .AnyAsync(tt => tt.TaskItemId == dto.TaskItemId && tt.TagId == dto.TagId);

        if (exists) return BadRequest("Tag is already assigned to this task.");

        var taskTag = new TaskTag
        {
            TaskItemId = dto.TaskItemId,
            TagId = dto.TagId
        };

        _context.TaskTags.Add(taskTag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTaskTags), new { taskItemId = taskTag.TaskItemId, tagId = taskTag.TagId },
            new TaskTagDto { TaskItemId = taskTag.TaskItemId, TagId = taskTag.TagId });
    }

    // DELETE: api/TaskTags
    [HttpDelete]
    public async Task<IActionResult> RemoveTaskTag(Guid taskItemId, Guid tagId)
    {
        var taskTag = await _context.TaskTags.FindAsync(taskItemId, tagId);
        if (taskTag == null) return NotFound();

        _context.TaskTags.Remove(taskTag);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

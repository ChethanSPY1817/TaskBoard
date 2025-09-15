using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.TaskAssignments;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskAssignmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public TaskAssignmentsController(ApplicationDbContext context) => _context = context;

    // GET: api/TaskAssignments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskAssignmentDto>>> GetAssignments()
    {
        var assignments = await _context.TaskAssignments
            .AsNoTracking()
            .Select(a => new TaskAssignmentDto
            {
                Id = a.Id,
                TaskItemId = a.TaskItemId,
                AssignedToUserId = a.AssignedToUserId,
                AssignedByUserId = a.AssignedByUserId,
                AssignedAt = a.AssignedAt,
                Comment = a.Comment
            })
            .ToListAsync();

        return Ok(assignments);
    }

    // GET: api/TaskAssignments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskAssignmentDto>> GetAssignment(Guid id)
    {
        var assignment = await _context.TaskAssignments
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new TaskAssignmentDto
            {
                Id = a.Id,
                TaskItemId = a.TaskItemId,
                AssignedToUserId = a.AssignedToUserId,
                AssignedByUserId = a.AssignedByUserId,
                AssignedAt = a.AssignedAt,
                Comment = a.Comment
            })
            .FirstOrDefaultAsync();

        if (assignment == null) return NotFound();
        return Ok(assignment);
    }

    // POST: api/TaskAssignments
    [HttpPost]
    public async Task<ActionResult<TaskAssignmentDto>> CreateAssignment(CreateTaskAssignmentDto dto)
    {
        var assignment = new TaskAssignment
        {
            Id = Guid.NewGuid(),
            TaskItemId = dto.TaskItemId,
            AssignedToUserId = dto.AssignedToUserId,
            AssignedByUserId = dto.AssignedByUserId,
            Comment = dto.Comment,
            AssignedAt = DateTime.UtcNow
        };

        _context.TaskAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, new TaskAssignmentDto
        {
            Id = assignment.Id,
            TaskItemId = assignment.TaskItemId,
            AssignedToUserId = assignment.AssignedToUserId,
            AssignedByUserId = assignment.AssignedByUserId,
            AssignedAt = assignment.AssignedAt,
            Comment = assignment.Comment
        });
    }

    // PUT: api/TaskAssignments/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAssignment(Guid id, UpdateTaskAssignmentDto dto)
    {
        var assignment = await _context.TaskAssignments.FindAsync(id);
        if (assignment == null) return NotFound();

        assignment.AssignedToUserId = dto.AssignedToUserId ?? assignment.AssignedToUserId;
        assignment.Comment = dto.Comment ?? assignment.Comment;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/TaskAssignments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAssignment(Guid id)
    {
        var assignment = await _context.TaskAssignments.FindAsync(id);
        if (assignment == null) return NotFound();

        _context.TaskAssignments.Remove(assignment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

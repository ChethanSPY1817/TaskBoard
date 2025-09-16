using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.TaskAssignments;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class TaskAssignmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TaskAssignmentsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/TaskAssignments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskAssignmentDto>>> GetAssignments()
    {
        var assignments = await _context.TaskAssignments.AsNoTracking().ToListAsync();
        return Ok(_mapper.Map<List<TaskAssignmentDto>>(assignments));
    }

    // GET: api/TaskAssignments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskAssignmentDto>> GetAssignment(Guid id)
    {
        var assignment = await _context.TaskAssignments.FindAsync(id);
        if (assignment == null) return NotFound();

        return Ok(_mapper.Map<TaskAssignmentDto>(assignment));
    }

    // POST: api/TaskAssignments
    [HttpPost]
    [Authorize(Roles = "Manager,SuperAdmin")]
    public async Task<ActionResult<TaskAssignmentDto>> CreateAssignment(CreateTaskAssignmentDto dto)
    {
        var assignment = _mapper.Map<TaskAssignment>(dto);
        assignment.Id = Guid.NewGuid();
        assignment.AssignedAt = DateTime.UtcNow;

        _context.TaskAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, _mapper.Map<TaskAssignmentDto>(assignment));
    }

    // PUT: api/TaskAssignments/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,SuperAdmin")]
    public async Task<IActionResult> UpdateAssignment(Guid id, UpdateTaskAssignmentDto dto)
    {
        var assignment = await _context.TaskAssignments.FindAsync(id);
        if (assignment == null) return NotFound();

        _mapper.Map(dto, assignment); // only updates mapped fields
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/TaskAssignments/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,SuperAdmin")]
    public async Task<IActionResult> DeleteAssignment(Guid id)
    {
        var assignment = await _context.TaskAssignments.FindAsync(id);
        if (assignment == null) return NotFound();

        _context.TaskAssignments.Remove(assignment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
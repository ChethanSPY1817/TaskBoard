using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskBoard.Application.DTOs.ProjectMembers;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class ProjectMembersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectMembersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ProjectMembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectMemberDto>>> GetMembers()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            IQueryable<ProjectMember> query = _context.ProjectMembers.AsNoTracking();

            if (userRole == "Developer")
            {
                // Developers see only members of projects they belong to
                query = query.Where(pm => pm.Project.Members.Any(m => m.UserId == userId));
            }
            // Managers & SuperAdmin see all members

            var members = await query.ToListAsync();
            return Ok(_mapper.Map<List<ProjectMemberDto>>(members));
        }

        // GET: api/ProjectMembers/{projectId}/{userId}
        [HttpGet("{projectId}/{userId}")]
        public async Task<ActionResult<ProjectMemberDto>> GetMember(Guid projectId, Guid userId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var member = await _context.ProjectMembers.FindAsync(projectId, userId);
            if (member == null) return NotFound();

            if (userRole == "Developer" &&
                !await _context.ProjectMembers.AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == currentUserId))
            {
                return Forbid(); // Developer cannot see members of projects they are not part of
            }

            return Ok(_mapper.Map<ProjectMemberDto>(member));
        }

        // POST: api/ProjectMembers
        [HttpPost]
        [Authorize(Roles = "Manager,SuperAdmin")]
        public async Task<ActionResult<ProjectMemberDto>> CreateMember(CreateProjectMemberDto dto)
        {
            var exists = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == dto.ProjectId && pm.UserId == dto.UserId);

            if (exists) return BadRequest("Member already exists.");

            var member = _mapper.Map<ProjectMember>(dto);
            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMember),
                                   new { projectId = member.ProjectId, userId = member.UserId },
                                   _mapper.Map<ProjectMemberDto>(member));
        }

        // PUT: api/ProjectMembers/{projectId}/{userId}
        [HttpPut("{projectId}/{userId}")]
        [Authorize(Roles = "Manager,SuperAdmin")]
        public async Task<IActionResult> UpdateMember(Guid projectId, Guid userId, UpdateProjectMemberDto dto)
        {
            var member = await _context.ProjectMembers.FindAsync(projectId, userId);
            if (member == null) return NotFound();

            _mapper.Map(dto, member);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/ProjectMembers/{projectId}/{userId}
        [HttpDelete("{projectId}/{userId}")]
        [Authorize(Roles = "Manager,SuperAdmin")]
        public async Task<IActionResult> DeleteMember(Guid projectId, Guid userId)
        {
            var member = await _context.ProjectMembers.FindAsync(projectId, userId);
            if (member == null) return NotFound();

            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

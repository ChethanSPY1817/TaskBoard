using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.ProjectMembers;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var members = await _context.ProjectMembers.AsNoTracking().ToListAsync();
            return Ok(_mapper.Map<List<ProjectMemberDto>>(members));
        }

        // GET: api/ProjectMembers/{projectId}/{userId}
        [HttpGet("{projectId}/{userId}")]
        public async Task<ActionResult<ProjectMemberDto>> GetMember(Guid projectId, Guid userId)
        {
            var member = await _context.ProjectMembers
                .FindAsync(projectId, userId);

            if (member == null) return NotFound();

            return Ok(_mapper.Map<ProjectMemberDto>(member));
        }

        // POST: api/ProjectMembers
        [HttpPost]
        public async Task<ActionResult<ProjectMemberDto>> CreateMember(CreateProjectMemberDto dto)
        {
            var exists = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == dto.ProjectId && pm.UserId == dto.UserId);

            if (exists) return BadRequest("Member already exists.");

            var member = _mapper.Map<ProjectMember>(dto);
            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMember), new { projectId = member.ProjectId, userId = member.UserId },
                                   _mapper.Map<ProjectMemberDto>(member));
        }

        // PUT: api/ProjectMembers/{projectId}/{userId}
        [HttpPut("{projectId}/{userId}")]
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

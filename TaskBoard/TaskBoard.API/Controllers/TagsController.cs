using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.Tags;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TagsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/Tags
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
    {
        var tags = await _context.Tags.AsNoTracking().ToListAsync();
        return Ok(_mapper.Map<List<TagDto>>(tags));
    }

    // GET: api/Tags/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(Guid id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        return Ok(_mapper.Map<TagDto>(tag));
    }

    // POST: api/Tags
    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto dto)
    {
        var tag = _mapper.Map<Tag>(dto);
        tag.Id = Guid.NewGuid();

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, _mapper.Map<TagDto>(tag));
    }

    // PUT: api/Tags/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(Guid id, UpdateTagDto dto)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        _mapper.Map(dto, tag);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Tags/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(Guid id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

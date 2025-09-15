using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Application.DTOs.Tags;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public TagsController(ApplicationDbContext context) => _context = context;

    // GET: api/Tags
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
    {
        var tags = await _context.Tags
            .AsNoTracking()
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                ColorHex = t.ColorHex
            }).ToListAsync();

        return Ok(tags);
    }

    // GET: api/Tags/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(Guid id)
    {
        var tag = await _context.Tags
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                ColorHex = t.ColorHex
            }).FirstOrDefaultAsync();

        if (tag == null) return NotFound();
        return Ok(tag);
    }

    // POST: api/Tags
    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto dto)
    {
        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            ColorHex = dto.ColorHex
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            ColorHex = tag.ColorHex
        });
    }

    // PUT: api/Tags/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(Guid id, UpdateTagDto dto)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        tag.Name = dto.Name;
        tag.ColorHex = dto.ColorHex;
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

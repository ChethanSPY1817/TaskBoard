using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.DTOs.Roles;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Repositories.RoleRepository;

namespace TaskBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")] // Only SuperAdmin can access any endpoint
public class RolesController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public RolesController(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        var roles = await _roleRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<RoleDto>>(roles));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetRole(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) return NotFound();

        return Ok(_mapper.Map<RoleDto>(role));
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleDto dto)
    {
        var role = _mapper.Map<Role>(dto);
        role.Id = Guid.NewGuid();

        await _roleRepository.AddAsync(role);

        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, _mapper.Map<RoleDto>(role));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(Guid id, UpdateRoleDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) return NotFound();

        _mapper.Map(dto, role); // Map update DTO onto existing entity
        await _roleRepository.UpdateAsync(role);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null) return NotFound();

        await _roleRepository.DeleteAsync(role);
        return NoContent();
    }
}
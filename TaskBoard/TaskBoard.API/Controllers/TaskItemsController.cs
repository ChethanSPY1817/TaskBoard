using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.DTOs.TaskItems;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Repositories.TaskItemRepository;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class TaskItemsController : ControllerBase
    {
        private readonly ITaskItemRepository _repository;
        private readonly IMapper _mapper;

        public TaskItemsController(ITaskItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/TaskItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAll()
        {
            var tasks = await _repository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }

        // GET: api/TaskItems/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDto>> GetById(Guid id)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task == null) return NotFound();

            return Ok(_mapper.Map<TaskItemDto>(task));
        }

        // POST: api/TaskItems
        [HttpPost]
        [Authorize(Roles = "Manager,SuperAdmin")]
        public async Task<ActionResult<TaskItemDto>> Create(CreateTaskItemDto dto)
        {
            var task = _mapper.Map<TaskItem>(dto);
            task.Id = Guid.NewGuid(); // Generate new ID

            await _repository.AddAsync(task);

            return CreatedAtAction(nameof(GetById), new { id = task.Id }, _mapper.Map<TaskItemDto>(task));
        }

        // PUT: api/TaskItems/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,SuperAdmin")]
        public async Task<IActionResult> Update(Guid id, UpdateTaskItemDto dto)
        {
            var existingTask = await _repository.GetByIdAsync(id);
            if (existingTask == null) return NotFound();

            _mapper.Map(dto, existingTask); // Map only non-null properties
            await _repository.UpdateAsync(existingTask);

            return NoContent();
        }

        // DELETE: api/TaskItems/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,SuperAdmin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task == null) return NotFound();

            await _repository.DeleteAsync(task);
            return NoContent();
        }
    }
}

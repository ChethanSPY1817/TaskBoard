using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.DTOs.TaskTags;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Repositories.TaskTagRepository;

namespace TaskBoard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskTagsController : ControllerBase
    {
        private readonly ITaskTagRepository _repository;
        private readonly IMapper _mapper;

        public TaskTagsController(ITaskTagRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all task-tag assignments
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskTagDto>>> GetAll()
        {
            var taskTags = await _repository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<TaskTagDto>>(taskTags));
        }

        /// <summary>
        /// Assign a tag to a task
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TaskTagDto>> Add(CreateTaskTagDto dto)
        {
            // Prevent duplicates
            var existing = await _repository.GetByIdAsync(dto.TaskItemId, dto.TagId);
            if (existing != null) return BadRequest("Tag is already assigned to this task.");

            var taskTag = _mapper.Map<TaskTag>(dto);
            await _repository.AddAsync(taskTag);

            return CreatedAtAction(nameof(GetAll),
                new { taskItemId = taskTag.TaskItemId, tagId = taskTag.TagId },
                _mapper.Map<TaskTagDto>(taskTag));
        }

        /// <summary>
        /// Remove a tag from a task
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Remove(Guid taskItemId, Guid tagId)
        {
            var taskTag = await _repository.GetByIdAsync(taskItemId, tagId);
            if (taskTag == null) return NotFound();

            await _repository.DeleteAsync(taskTag);
            return NoContent();
        }
    }
}

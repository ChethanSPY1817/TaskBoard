using TaskBoard.Domain.Entities;
using TaskStatus = TaskBoard.Domain.Entities.TaskStatus;

namespace TaskBoard.Application.DTOs.TaskItems
{

    public class CreateTaskItemDto
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid AssignedToUserId { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
    }
}

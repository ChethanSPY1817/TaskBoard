using TaskBoard.Domain.Entities;
using TaskStatus = TaskBoard.Domain.Entities.TaskStatus;

namespace TaskBoard.Application.DTOs.TaskItems
{
    public class UpdateTaskItemDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public TaskStatus? Status { get; set; }       // fully qualified or using statement
        public TaskPriority? Priority { get; set; }   // fully qualified or using statement
    }
}

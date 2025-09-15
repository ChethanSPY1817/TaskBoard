using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBoard.Domain.Entities;
using TaskStatus = TaskBoard.Domain.Entities.TaskStatus;

namespace TaskBoard.Application.DTOs.TaskItems
{
    public class TaskItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
    }
}

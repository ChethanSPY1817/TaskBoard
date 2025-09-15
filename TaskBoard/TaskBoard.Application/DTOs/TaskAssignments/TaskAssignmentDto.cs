using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Application.DTOs.TaskAssignments
{
    public class TaskAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid TaskItemId { get; set; }
        public Guid AssignedToUserId { get; set; }
        public Guid AssignedByUserId { get; set; }
        public DateTime AssignedAt { get; set; }
        public string? Comment { get; set; }
    }
}

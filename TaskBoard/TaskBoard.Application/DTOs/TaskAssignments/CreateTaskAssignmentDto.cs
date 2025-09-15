namespace TaskBoard.Application.DTOs.TaskAssignments
{
    public class CreateTaskAssignmentDto
    {
        public Guid TaskItemId { get; set; }
        public Guid AssignedToUserId { get; set; }
        public Guid AssignedByUserId { get; set; }
        public string? Comment { get; set; }
    }
}

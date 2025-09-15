namespace TaskBoard.Application.DTOs.TaskAssignments
{
    public class UpdateTaskAssignmentDto
    {
        public Guid? AssignedToUserId { get; set; }
        public string? Comment { get; set; }
    }
}

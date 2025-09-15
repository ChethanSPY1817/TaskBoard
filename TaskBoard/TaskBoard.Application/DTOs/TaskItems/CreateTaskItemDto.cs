namespace TaskBoard.Application.DTOs.TaskItems
{
    public class CreateTaskItemDto
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string Status { get; set; } = "New";
        public string Priority { get; set; } = "Medium";
    }
}

namespace TaskBoard.Application.DTOs.TaskItems
{
    public class UpdateTaskItemDto
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string Status { get; set; } = "New";
        public string Priority { get; set; } = "Medium";
    }
}

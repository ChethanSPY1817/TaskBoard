namespace TaskBoard.Application.DTOs.TaskTags
{
    public class UpdateTaskTagDto
    {
        public Guid TaskItemId { get; set; }
        public Guid TagId { get; set; }
    }
}

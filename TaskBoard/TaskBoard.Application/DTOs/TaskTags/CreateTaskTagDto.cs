namespace TaskBoard.Application.DTOs.TaskTags
{
    public class CreateTaskTagDto
    {
        public Guid TaskItemId { get; set; }
        public Guid TagId { get; set; }
    }
}

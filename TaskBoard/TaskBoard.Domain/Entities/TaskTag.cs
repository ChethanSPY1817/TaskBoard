namespace TaskBoard.Domain.Entities;

public class TaskTag
{
    public Guid TaskItemId { get; set; }
    public Guid TagId { get; set; }

    // Navigation
    public TaskItem TaskItem { get; set; } = default!;
    public Tag Tag { get; set; } = default!;
}

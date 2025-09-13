namespace TaskBoard.Domain.Entities;

public class TaskAssignment
{
    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }
    public Guid AssignedToUserId { get; set; }
    public Guid AssignedByUserId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public string? Comment { get; set; }

    // Navigation
    public TaskItem TaskItem { get; set; } = default!;
    public User AssignedToUser { get; set; } = default!;
    public User AssignedByUser { get; set; } = default!;
}

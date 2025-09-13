namespace TaskBoard.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string Status { get; set; } = "New";
    public string Priority { get; set; } = "Medium";

    // Navigation
    public Project Project { get; set; } = default!;
    public User? AssignedToUser { get; set; }
    public ICollection<TaskAssignment> Assignments { get; set; } = new List<TaskAssignment>();
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}

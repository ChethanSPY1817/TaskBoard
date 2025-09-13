namespace TaskBoard.Domain.Entities;

public enum TaskStatus { Todo = 0, InProgress = 1, Done = 2 }
public enum TaskPriority { Low = 0, Medium = 1, High = 2 }

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    // Relation to Project
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    // Assigned to a User (optional)
    public Guid? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    // Navigation
    public ICollection<TaskAssignment> Assignments { get; set; } = new List<TaskAssignment>();
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}

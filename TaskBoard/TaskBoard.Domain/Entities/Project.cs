namespace TaskBoard.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }

    // Navigation
    public User Owner { get; set; } = default!;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
}

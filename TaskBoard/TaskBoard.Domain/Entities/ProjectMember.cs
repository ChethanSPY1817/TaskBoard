namespace TaskBoard.Domain.Entities;

public class ProjectMember
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }

    // Navigation
    public Project Project { get; set; } = default!;
    public User User { get; set; } = default!;
}

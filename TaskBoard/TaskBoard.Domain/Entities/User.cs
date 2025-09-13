namespace TaskBoard.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;

    // 🔹 Foreign Key to Role
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;

    // Navigation
    public UserProfile? Profile { get; set; }
    public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
}

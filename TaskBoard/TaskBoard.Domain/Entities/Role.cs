namespace TaskBoard.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!; // "Admin", "Manager", "Developer"

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
}

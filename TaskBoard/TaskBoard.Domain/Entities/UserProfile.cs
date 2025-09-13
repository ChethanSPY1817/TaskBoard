namespace TaskBoard.Domain.Entities;

public class UserProfile
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Address { get; set; }

    // Navigation
    public User User { get; set; } = default!;
}

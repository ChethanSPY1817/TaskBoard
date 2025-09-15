namespace TaskBoard.Application.DTOs.UserProfiles
{
    public class CreateUserProfileDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}

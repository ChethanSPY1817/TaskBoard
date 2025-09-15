namespace TaskBoard.Application.DTOs.UserProfiles
{
    public class UpdateUserProfileDto
    {
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}

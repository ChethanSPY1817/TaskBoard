namespace TaskBoard.Application.DTOs.Users
{
    public class UpdateUserDto
    {
        public string? Email { get; set; }
        public Guid? RoleId { get; set; } // nullable for optional update
        public string? Password { get; set; } // add this
    }

}

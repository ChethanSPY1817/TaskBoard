namespace TaskBoard.Application.DTOs.Users
{
    public class CreateUserDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public Guid RoleId { get; set; }
    }
}

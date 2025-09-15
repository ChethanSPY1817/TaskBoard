namespace TaskBoard.Application.DTOs.Auth
{
    public class RegisterUserDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Guid RoleId { get; set; }  // Changed from Role name to RoleId
    }



}

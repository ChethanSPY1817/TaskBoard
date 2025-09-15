using System;

namespace TaskBoard.Domain.Entities
{
    public class PlainTextPassword
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }      // FK to User
        public string Password { get; set; }  // Plain-text password (unsafe!)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }        // Navigation property
    }
}

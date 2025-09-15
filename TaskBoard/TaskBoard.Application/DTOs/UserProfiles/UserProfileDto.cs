using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Application.DTOs.UserProfiles
{
    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}

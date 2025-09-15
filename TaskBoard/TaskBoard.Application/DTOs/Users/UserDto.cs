using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Application.DTOs.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public Guid RoleId { get; set; }
    }
}

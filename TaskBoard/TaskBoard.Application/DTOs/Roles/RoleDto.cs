using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Application.DTOs.Roles
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
    }
}

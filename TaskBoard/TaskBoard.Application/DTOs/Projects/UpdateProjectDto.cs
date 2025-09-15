using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Application.DTOs.Projects
{
    public class UpdateProjectDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}

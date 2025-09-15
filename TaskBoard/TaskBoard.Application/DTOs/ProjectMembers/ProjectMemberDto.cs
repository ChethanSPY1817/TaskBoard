using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Application.DTOs.ProjectMembers
{
    public class ProjectMemberDto
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
    }
}

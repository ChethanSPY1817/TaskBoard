using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Application.DTOs.TaskTags
{
    public class TaskTagDto
    {
        public Guid TaskItemId { get; set; }
        public Guid TagId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Application.DTOs.Tags
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string ColorHex { get; set; } = "#000000";
    }
}

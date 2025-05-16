using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Application.DTOs
{
    public class JobResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Id_item { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
    }
}

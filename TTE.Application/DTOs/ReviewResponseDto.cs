using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Application.DTOs
{
    public class ReviewResponseDto
    {
        public string User { get; set; } = string.Empty;
        public string Review { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}

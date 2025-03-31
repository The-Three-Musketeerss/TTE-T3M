using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Application.DTOs
{
    public class JobReviewRequestDto
    {
        [Required]
        public string Action { get; set; } = string.Empty;

    }
}

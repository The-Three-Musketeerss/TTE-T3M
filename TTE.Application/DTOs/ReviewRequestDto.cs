using System.ComponentModel.DataAnnotations;
using TTE.Commons.Constants;

namespace TTE.Application.DTOs
{
    public class ReviewRequestDto
    {
        [Range(1, 5, ErrorMessage = ValidationMessages.MESSAGE_RATING_NOT_VALID )]
        public int Rating { get; set; }

        [Required]
        public string Review { get; set; }
    }
}

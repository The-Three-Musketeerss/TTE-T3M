using System.ComponentModel.DataAnnotations;
using TTE.Commons.Validators;

namespace TTE.Application.DTOs
{
    public class InventoryRequestDto
    {
        [RequiredFieldValidator]
        [Range(0, int.MaxValue)]
        public int Total { get; set; }
        [RequiredFieldValidator]
        [Range(0, int.MaxValue)]
        public int Available { get; set; }
    }
}

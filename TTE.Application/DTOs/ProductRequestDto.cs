using TTE.Commons.Validators;

namespace TTE.Application.DTOs
{
    public class ProductRequestDto
    {
        [RequiredFieldValidator]
        public string Title { get; set; } = string.Empty;

        [RequiredFieldValidator]
        public decimal Price { get; set; }

        [RequiredFieldValidator]
        public string Description { get; set; } = string.Empty;

        [RequiredFieldValidator]
        public string Category { get; set; } = string.Empty;

        [RequiredFieldValidator]
        public string Image { get; set; } = string.Empty;

        public RatingDto Rating { get; set; }
        public InventoryDto Inventory { get; set; }
    }

}


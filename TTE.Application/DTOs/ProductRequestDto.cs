using TTE.Infrastructure.Models;
using TTE.Commons.Validators;

namespace TTE.Application.DTOs
{
    public class ProductUpdateRequestDto
    {
        public string? Title { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Image { get; set; }
        public InventoryRequestDto? Inventory { get; set; }

    }

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
        public InventoryDto Inventory { get; set; }
    }

}


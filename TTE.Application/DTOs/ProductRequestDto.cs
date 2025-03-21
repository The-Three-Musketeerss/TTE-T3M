using TTE.Infrastructure.Models;

namespace TTE.Application.DTOs
{
    public class ProductRequestDto
    {
        public string? Title { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Image { get; set; }
        public InventoryRequestDto? Inventory { get; set; }

    }
}

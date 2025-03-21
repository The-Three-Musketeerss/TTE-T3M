using TTE.Infrastructure.Models;

namespace TTE.Application.DTOs
{
    public class ProductRequestDto
    {
        public int? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;

        public Inventory Inventory { get; set; } = new Inventory();
    }
}

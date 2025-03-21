namespace TTE.Infrastructure.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public bool Approved { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public Inventory Inventory { get; set; }

    }
}

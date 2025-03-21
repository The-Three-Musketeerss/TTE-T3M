namespace TTE.Infrastructure.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int Total { get; set; }
        public int Available { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}

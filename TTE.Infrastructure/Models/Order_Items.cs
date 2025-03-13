using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Order_Items
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        //Order FK
        public int OrderId { get; set; }
        public Order Order { get; set; }

        //Product FK
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}

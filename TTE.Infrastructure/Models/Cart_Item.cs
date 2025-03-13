using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Cart_Item
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        //Product FK
        public int ProductId { get; set; }
        public Product Product { get; set; }

        //Cart FK
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        //User FK
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

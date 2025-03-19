using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Wishlist_Item
    {
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}

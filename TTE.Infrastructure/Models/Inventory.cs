using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int Total { get; set; }
        public int Available { get; set; }

        //Product FK
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}

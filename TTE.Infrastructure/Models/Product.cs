using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //Category FK
        public int CategoryId { get; set; }
        public Category Category { get; set; } = new Category();

    }
}

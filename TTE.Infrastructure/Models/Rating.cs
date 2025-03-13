using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int Rate { get; set; }
        //Product FK
        public int ProductId { get; set; }
        public Product Product { get; set; }
        //User FK
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

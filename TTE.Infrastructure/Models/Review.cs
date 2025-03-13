using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        //Product FK
        public int ProductId { get; set; }
        public Product Product { get; set; }

        //User FK
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

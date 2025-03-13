using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Job
    {
        public int Id { get; set; }
        public enum Type { Product, Category }
        public int Item_id { get; set; }
        public enum Operatipon { Create, Delete }
        public enum Status { Approved, Declined }
        public DateTime CreatedAt { get; set; }
    }
}

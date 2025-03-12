using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Approved { get; set; }
    }
}

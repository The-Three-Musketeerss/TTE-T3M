using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Optional { get; set; } = string.Empty;

        //User FK
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

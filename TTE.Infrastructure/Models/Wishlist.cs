using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public string Products_Id { get; set; } = string.Empty;

        //User FK
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

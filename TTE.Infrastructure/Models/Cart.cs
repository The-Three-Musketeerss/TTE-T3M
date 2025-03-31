using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public decimal Total_before_discount { get; set; }
        public decimal Total_after_discount { get; set; }
        public decimal ShippingCost { get; set; }
        //User FK
        public int UserId { get; set; }
        public User User { get; set; }

        //Coupon FK
        public int? CouponId { get; set; }
        public Coupon Coupon { get; set; }
    }
}

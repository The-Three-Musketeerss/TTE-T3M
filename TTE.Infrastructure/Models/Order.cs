using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Infrastructure.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Total_before_discount { get; set; }
        public decimal Total_after_discount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal FinalTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        //User FK
        public int UserId { get; set; }
        public User User { get; set; }

        //Address FK
        public int AddressId { get; set; }
        public Address Address { get; set; }

        //Coupon FK
        public int CouponId { get; set; }
        public Coupon Coupon { get; set; }
    }
}

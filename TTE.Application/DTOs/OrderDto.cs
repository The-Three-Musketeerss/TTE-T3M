namespace TTE.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? CouponId { get; set; }
        public decimal TotalBeforeDiscount { get; set; }
        public decimal TotalAfterDiscount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal FinalTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "Paid";
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}

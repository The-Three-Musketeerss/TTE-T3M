namespace TTE.Application.DTOs
{
    public class CartResponseDto
    {
        public int UserId { get; set; }
        public List<CartItemResponseDto> ShoppingCart { get; set; } = new();
        public CouponAppliedDto? CouponApplied { get; set; }
        public decimal TotalBeforeDiscount { get; set; }
        public decimal TotalAfterDiscount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal FinalTotal { get; set; }
    }
}

using TTE.Commons.Validators;

namespace TTE.Application.DTOs
{
    public class CartItemRequestDto
    {
        [RequiredFieldValidator]
        public int ProductId { get; set; }
        [RequiredFieldValidator]
        public int Quantity { get; set; }
    }
}

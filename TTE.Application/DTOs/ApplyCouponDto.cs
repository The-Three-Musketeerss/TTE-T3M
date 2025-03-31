using TTE.Commons.Validators;

namespace TTE.Application.DTOs
{
    public class ApplyCouponDto
    {
        [RequiredFieldValidator]
        public string Code { get; set; } = string.Empty;
    }
}

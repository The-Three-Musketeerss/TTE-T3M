using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Commons.Validators;

namespace TTE.Application.DTOs
{
    public class CouponRequestDto
    {
        [RequiredFieldValidator]
        public string Code { get; set; } = string.Empty;
        [RequiredFieldValidator]
        public decimal Discount { get; set; }
    }
}

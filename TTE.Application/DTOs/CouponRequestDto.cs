﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTE.Application.DTOs
{
    public class CouponRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal Discount { get; set; }
    }
}

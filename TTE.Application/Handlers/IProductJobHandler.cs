﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;
using TTE.Infrastructure.Models;

namespace TTE.Application.Handlers
{
    public interface IProductJobHandler
    {
        Task<GenericResponseDto<string>> Handle(Job job, Product product, string action);
    }
}

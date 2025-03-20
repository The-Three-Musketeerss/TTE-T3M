using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Infrastructure.Models;

namespace TTE.Application.DTOs
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public bool Approved { get; set; }

        //Category FK
        public int CategoryId { get; set; }
    }
}

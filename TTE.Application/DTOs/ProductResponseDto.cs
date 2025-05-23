﻿namespace TTE.Application.DTOs
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;

        public RatingDto Rating { get; set; } = new RatingDto();
        public DateTime CreatedAt { get; set; }
        public InventoryDto Inventory { get; set; } = new InventoryDto();
    }
}

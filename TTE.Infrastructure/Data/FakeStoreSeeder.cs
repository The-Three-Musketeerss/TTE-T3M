using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TTE.Infrastructure.Models;

namespace TTE.Infrastructure.Data
{
    public class FakeStoreSeeder
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger<FakeStoreSeeder> _logger;
        private readonly bool _enableSeeding;

        public FakeStoreSeeder(AppDbContext context, HttpClient httpClient, ILogger<FakeStoreSeeder> logger, bool enableSeeding)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
            _enableSeeding = enableSeeding;
        }

        public async Task SeedDataAsync()
        {
            if (!_enableSeeding)
            {
                return;
            }

            if (await _context.Products.AnyAsync() && await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Database already seeded.");
                return;
            }

            _logger.LogInformation("Fetching products from FakeStore API...");
            var response = await _httpClient.GetAsync("https://fakestoreapi.com/products");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch data from FakeStore API.");
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<FakeStoreProduct>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (products == null || !products.Any())
            {
                _logger.LogWarning("No products found in FakeStore API response.");
                return;
            }

            var categories = products.Select(p => p.Category).Distinct().ToList();

            var existingCategories = await _context.Categories.ToListAsync();
            foreach (var categoryName in categories)
            {
                if (!existingCategories.Any(c => c.Name == categoryName))
                {
                    _context.Categories.Add(new Category { Name = categoryName, Approved = true });
                }
            }
            await _context.SaveChangesAsync();

            var categoryDictionary = await _context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

            var productEntities = new List<Product>();

            foreach (var productDto in products)
            {
                if (!await _context.Products.AnyAsync(p => p.Title == productDto.Title))
                {
                    var productEntity = new Product
                    {
                        Title = productDto.Title,
                        Description = productDto.Description,
                        Price = productDto.Price,
                        Image = productDto.Image,
                        CategoryId = categoryDictionary[productDto.Category],
                        Approved = true
                    };

                    productEntities.Add(productEntity);
                    _context.Products.Add(productEntity);
                }
            }

            await _context.SaveChangesAsync();

            foreach (var product in productEntities)
            {
                var inventoryItem = new Inventory
                {
                    ProductId = product.Id,
                    Total = 100,
                    Available = 100
                };
                _context.Inventory.Add(inventoryItem);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeded successfully without ratings.");
        }
    }
}

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTE.Seeding.DTOs;
using TTE.Seeding.Infrastructure;

namespace TTE.Seeding.Services;

public class FakeStoreSeeder
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly ILogger<FakeStoreSeeder> _logger;

    public FakeStoreSeeder(AppDbContext context, HttpClient httpClient, ILogger<FakeStoreSeeder> logger)
    {
        _context = context;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SeedDataAsync()
    {
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
                if (categoryDictionary.TryGetValue(productDto.Category, out var categoryId))
                {
                    var productEntity = new Product
                    {
                        Title = productDto.Title,
                        Description = productDto.Description,
                        Price = productDto.Price,
                        Image = productDto.Image,
                        CategoryId = categoryId,
                        Approved = true
                    };

                    productEntities.Add(productEntity);
                    _context.Products.Add(productEntity);
                }
                else
                {
                    _logger.LogWarning($"Category '{productDto.Category}' not found in dictionary.");
                }
            }
        }

        await _context.SaveChangesAsync();

        foreach (var product in productEntities)
        {
            if (!await _context.Inventory.AnyAsync(i => i.ProductId == product.Id))
            {
                var inventoryItem = new Inventory
                {
                    ProductId = product.Id,
                    Total = 100,
                    Available = 100
                };

                _context.Inventory.Add(inventoryItem);
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Database seeded successfully without ratings.");
    }

}
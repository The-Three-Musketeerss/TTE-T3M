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
        if (await _context.Products.AnyAsync())
        {
            _logger.LogInformation("Database already seeded.");
            return;
        }

        var response = await _httpClient.GetAsync("https://fakestoreapi.com/products");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch data from FakeStore API.");
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<FakeStoreProduct>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (products == null || !products.Any())
        {
            _logger.LogWarning("No products found in response.");
            return;
        }

        var categories = products.Select(p => p.Category).Distinct().ToList();

        foreach (var categoryName in categories)
        {
            if (!await _context.Categories.AnyAsync(c => c.Name == categoryName))
                _context.Categories.Add(new Category { Name = categoryName });
        }

        await _context.SaveChangesAsync();

        var categoryDict = await _context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

        foreach (var productDto in products)
        {
            var product = new Product
            {
                Title = productDto.Title,
                Description = productDto.Description,
                Price = productDto.Price,
                Image = productDto.Image,
                CategoryId = categoryDict[productDto.Category]
            };
            _context.Products.Add(product);
        }

        await _context.SaveChangesAsync();

        foreach (var product in await _context.Products.ToListAsync())
        {
            _context.Inventory.Add(new Inventory
            {
                ProductId = product.Id,
                Total = 100,
                Available = 100
            });
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Database seeded successfully.");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TTE.Infrastructure.Models;


namespace TTE.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Category> Categories { get; set; }
        public DbSet<Models.Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
        new Category { Id = 1, Name = "Electronics", Approved = true },
        new Category { Id = 2, Name = "Books", Approved = true }
    );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Smartphone",
                    Price = 299.99m,
                    Description = "Latest model smartphone.",
                    Image = "smartphone.jpg",
                    Approved = true,
                    CategoryId = 1 // References Electronics
                },
                new Product
                {
                    Id = 2,
                    Title = "Novel",
                    Price = 19.99m,
                    Description = "Bestselling novel.",
                    Image = "novel.jpg",
                    Approved = true,
                    CategoryId = 2 // References Books
                }
            );
        }

    }
}

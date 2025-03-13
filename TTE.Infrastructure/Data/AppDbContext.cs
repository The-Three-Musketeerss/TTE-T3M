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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Approved = true },
                new Category { Id = 2, Name = "Furniture", Approved = false }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Title = "Laptop", Price = 1200.99M, CategoryId = 1, Description = "", Image = "", Approved = true },
                new Product { Id = 2, Title = "Table", Price = 200.50M, CategoryId = 2, Description = "", Image = "", Approved = false }
            );
        }
    }
}

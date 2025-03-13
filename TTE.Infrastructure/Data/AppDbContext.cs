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
        public DbSet<Models.Job> Jobs { get; set; }
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Role> Roles { get; set; }
        public DbSet<Models.Cart> Carts { get; set; }
        public DbSet<Models.Cart_Item> Cart_Items { get; set; }
        public DbSet<Models.Order> Orders { get; set; }
        public DbSet<Models.Order_Items> Order_Items { get; set; }
        public DbSet<Models.Address> Addresses { get; set; }
        public DbSet<Models.Coupon> Coupons { get; set; }
        public DbSet<Models.Inventory> Inventory { get; set; }
        public DbSet<Models.Review> Reviews { get; set; }
        public DbSet<Models.Wishlist> Wishlists { get; set; }
        public DbSet<Models.Rating> Ratings { get; set; }
        public DbSet<Models.SecurityQuestion> SecurityQuestions { get; set; }

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

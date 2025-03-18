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
            modelBuilder.Entity<Role>().HasData(
                new Role {Id = 1, Name = "Admin" },
                new Role {Id = 2, Name = "Employee" },
                new Role {Id = 3, Name = "Shopper" }
            );
            modelBuilder.Entity<SecurityQuestion>().HasData(
                new SecurityQuestion { Id = 1, Question = "What is your favorite color?" },
                new SecurityQuestion { Id = 2, Question = "What is your favorite food?" },
                new SecurityQuestion { Id = 3, Question = "What is your favorite movie?" }
            );
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon {Id = 1, Code = "10OFF", Discount = 10 },
                new Coupon { Id = 2, Code = "20OFF", Discount = 20 },
                new Coupon { Id = 3, Code = "30OFF", Discount = 30 }
            );


            modelBuilder.Entity<User>()
                .HasOne(e => e.SecurityQuestion)
                .WithMany()
                .HasForeignKey(e => e.SecurityQuestionId)
                .IsRequired(false);
            modelBuilder.Entity<User>()
                .HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .IsRequired();
            modelBuilder.Entity<Rating>()
                .HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .IsRequired();
            modelBuilder.Entity<Rating>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();
            modelBuilder.Entity<Review>()
                .HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .IsRequired();
            modelBuilder.Entity<Review>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();
            modelBuilder.Entity<Wishlist>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();
            modelBuilder.Entity<Product>()
                .HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .IsRequired();
            modelBuilder.Entity<Inventory>()
                .HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .IsRequired();
            modelBuilder.Entity<Cart>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();
            modelBuilder.Entity<Cart>()
                .HasOne(e => e.Coupon)
                .WithMany()
                .HasForeignKey(e => e.CouponId)
                .IsRequired();
            modelBuilder.Entity<Cart_Item>()
                .HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .IsRequired();
            modelBuilder.Entity<Cart_Item>()
                .HasOne(e => e.Cart)
                .WithMany()
                .HasForeignKey(e => e.CartId)
                .IsRequired();
            modelBuilder.Entity<Cart_Item>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();
            modelBuilder.Entity<Order>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();
            modelBuilder.Entity<Order>()
                .HasOne(e => e.Address)
                .WithMany()
                .HasForeignKey(e => e.AddressId)
                .IsRequired();
            modelBuilder.Entity<Order>()
                .HasOne(e => e.Coupon)
                .WithMany()
                .HasForeignKey(e => e.CouponId)
                .IsRequired();
            modelBuilder.Entity<Order_Items>()
                .HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .IsRequired();
            modelBuilder.Entity<Order_Items>()
                .HasOne(e => e.Order)
                .WithMany()
                .HasForeignKey(e => e.OrderId)
                .IsRequired();
            modelBuilder.Entity<Job>()
                .Property(e => e.Type)
                .IsRequired();
            modelBuilder.Entity<Job>()
                .Property(e => e.Item_id)
                .IsRequired();
        }
    }
}

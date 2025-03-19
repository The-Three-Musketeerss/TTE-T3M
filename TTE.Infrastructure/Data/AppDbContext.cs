using Microsoft.EntityFrameworkCore;
using TTE.Commons.Constants;
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
        public DbSet<Models.Order_Item> Order_Items { get; set; }
        public DbSet<Models.Address> Addresses { get; set; }
        public DbSet<Models.Coupon> Coupons { get; set; }
        public DbSet<Models.Inventory> Inventory { get; set; }
        public DbSet<Models.Review> Reviews { get; set; }
        public DbSet<Models.Wishlist> Wishlists { get; set; }
        public DbSet<Models.Wishlist_Item> Wishlist_Items { get; set; }
        public DbSet<Models.Rating> Ratings { get; set; }
        public DbSet<Models.SecurityQuestion> SecurityQuestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = AppConstants.ADMIN},
                new Role { Id = 2, Name = AppConstants.EMPLOYEE },
                new Role { Id = 3, Name = AppConstants.SHOPPER }
            );
            modelBuilder.Entity<SecurityQuestion>().HasData(
                new SecurityQuestion { Id = 1, Question = AppConstants.SECURITY_QUESTION_1 },
                new SecurityQuestion { Id = 2, Question = AppConstants.SECURITY_QUESTION_2 },
                new SecurityQuestion { Id = 3, Question = AppConstants.SECURITY_QUESTION_3 }
            );
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon { Id = 1, Code = AppConstants.coupon_code_1, Discount = 10 },
                new Coupon { Id = 2, Code = AppConstants.coupon_code_2, Discount = 20 },
                new Coupon { Id = 3, Code = AppConstants.coupon_code_3, Discount = 30 }
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
                .IsRequired(false);
            modelBuilder.Entity<Cart_Item>()
                .HasKey(ci => new { ci.CartId, ci.ProductId });
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
                .IsRequired(false);
            modelBuilder.Entity<Order_Item>()
                .HasKey(oi => new { oi.OrderId, oi.ProductId });
            modelBuilder.Entity<Order_Item>()
                .HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .IsRequired();
            modelBuilder.Entity<Order_Item>()
                .HasOne(e => e.Order)
                .WithMany()
                .HasForeignKey(e => e.OrderId)
                .IsRequired();

            modelBuilder.Entity<Wishlist>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();
            modelBuilder.Entity<Wishlist_Item>()
                .HasKey(e => new { e.WishlistId, e.ProductId });
            modelBuilder.Entity<Wishlist_Item>()
                .HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .IsRequired();
            modelBuilder.Entity<Wishlist_Item>()
                .HasOne(e => e.Wishlist)
                .WithMany()
                .HasForeignKey(e => e.WishlistId)
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

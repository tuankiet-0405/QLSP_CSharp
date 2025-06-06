using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using THLTW.Models;
using THLTW.Services; // Add this for SearchLog

namespace THLTW.Data
{    // Updated to use ApplicationUser for AI training capabilities
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> 
    {        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Existing DbSets
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<ProductAdditionalImage> ProductAdditionalImages { get; set; } = null!;
        
        // New DbSets for AI training data
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<ProductReview> ProductReviews { get; set; } = null!;
        public DbSet<UserActivity> UserActivities { get; set; } = null!;
        public DbSet<ProductViewHistory> ProductViewHistories { get; set; } = null!;
        
        // AI Services DbSets
        public DbSet<SearchLog> SearchLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Existing configurations
            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<CartItem>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");
            
            // AI Training Data Configurations
            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");
                
            builder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");
              // Configure relationships for AI data
            builder.Entity<ProductReview>()
                .HasOne(pr => pr.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(pr => pr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<ProductReview>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<UserActivity>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.Activities)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<ProductViewHistory>()
                .HasOne(pvh => pvh.Product)
                .WithMany(p => p.ViewHistory)
                .HasForeignKey(pvh => pvh.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<ProductViewHistory>()
                .HasOne(pvh => pvh.User)
                .WithMany(u => u.ViewHistory)
                .HasForeignKey(pvh => pvh.UserId)
                .OnDelete(DeleteBehavior.SetNull);
                
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using BookShopingCartMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShopingCartMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Models.Book> Books { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Models.ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Genres
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, GenreName = "Science Fiction" },
                new Genre { Id = 2, GenreName = "Fantasy" },
                new Genre { Id = 3, GenreName = "Mystery" },
                new Genre { Id = 4, GenreName = "Romance" },
                new Genre { Id = 5, GenreName = "Horror" },
                new Genre { Id = 6, GenreName = "Biography" },
                new Genre { Id = 7, GenreName = "Other" }
            );

            // Seed OrderStatus
            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus { Id = 1, StatusName = "Pending" },
                new OrderStatus { Id = 2, StatusName = "processing" },
                new OrderStatus { Id = 3, StatusName = "shipped" },
                new OrderStatus { Id = 4, StatusName = "delivered" },
                new OrderStatus { Id = 5, StatusName = "cancelled" }
            );
        }

    }
}
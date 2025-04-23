using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Acme.SimpleTaskApp.Authorization.Roles;
using Acme.SimpleTaskApp.Authorization.Users;
using Acme.SimpleTaskApp.MultiTenancy;
using Acme.SimpleTaskApp.People;
using Acme.SimpleTaskApp.Entities.Products;
using Acme.SimpleTaskApp.Entities.Categories;
using Acme.SimpleTaskApp.Cart;
using Acme.SimpleTaskApp.Orders;

namespace Acme.SimpleTaskApp.EntityFrameworkCore
{
    public class SimpleTaskAppDbContext : AbpZeroDbContext<Tenant, Role, User, SimpleTaskAppDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Tasks.Tasks> Tasks { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public virtual DbSet<Cart.Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public SimpleTaskAppDbContext(DbContextOptions<SimpleTaskAppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cart.Cart>(b =>
            {
                b.ToTable("Carts");
                b.Property(x => x.UserId).IsRequired();
            });

            modelBuilder.Entity<CartItem>(b =>
            {
                b.ToTable("CartItems");
                b.Property(x => x.CartId).IsRequired();
                b.Property(x => x.ProductId).IsRequired();
                b.Property(x => x.Quantity).IsRequired();
                b.Property(x => x.Price).IsRequired();
            });

            modelBuilder.Entity<Order>(b =>
            {
                b.ToTable("Orders");
                b.Property(x => x.UserId).IsRequired();
                b.Property(x => x.UserName).IsRequired().HasMaxLength(256);
                b.Property(x => x.UserEmail).IsRequired().HasMaxLength(256);
                b.Property(x => x.TotalAmount).IsRequired();
                b.Property(x => x.Status).IsRequired();
            });

            modelBuilder.Entity<OrderItem>(b =>
            {
                b.ToTable("OrderItems");
                b.Property(x => x.OrderId).IsRequired();
                b.Property(x => x.ProductId).IsRequired();
                b.Property(x => x.ProductName).IsRequired().HasMaxLength(256);
                b.Property(x => x.Quantity).IsRequired();
                b.Property(x => x.Price).IsRequired();
            });

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

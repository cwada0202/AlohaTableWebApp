using Microsoft.EntityFrameworkCore;

namespace RestaurantWebApp.Models;

public class AlohaTableDbContext : DbContext
{
    public AlohaTableDbContext(DbContextOptions<AlohaTableDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Login> Logins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().ToTable("Categories");
        modelBuilder.Entity<Item>().ToTable("Items");
        modelBuilder.Entity<Order>().ToTable("Orders");
        modelBuilder.Entity<OrderDetail>().ToTable("OrderDetails");
        modelBuilder.Entity<Login>().ToTable("Logins");

        modelBuilder.Entity<Login>().HasIndex(x => x.UserName).IsUnique();
        modelBuilder.Entity<Login>().Property(x => x.UserPassword).IsRequired();
        modelBuilder.Entity<Login>().Property(x => x.UserName).IsRequired();

        modelBuilder.Entity<Item>().Property(x => x.ItemPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OrderDetail>().Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OrderDetail>().Property(x => x.Total).HasColumnType("decimal(18,2)");
    }
}

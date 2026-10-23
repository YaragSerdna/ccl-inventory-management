using Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

        public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products", "public");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(p => p.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
                entity.HasIndex(p => p.Code).IsUnique();
                entity.Property(p => p.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
                entity.Property(p => p.Description).HasColumnName("description").HasMaxLength(500);
                entity.Property(p => p.Stock).HasColumnName("stock").IsRequired();
                entity.Property(p => p.UnitPrice).HasColumnName("unit_price").HasPrecision(12, 2);
                entity.Property(p => p.IsActive).HasColumnName("is_active");
                entity.Property(p => p.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
                entity.Property(p => p.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<InventoryMovement>(entity =>
            {
                entity.ToTable("inventory_movements", "public");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(m => m.ProductId).HasColumnName("product_id");
                entity.Property(m => m.MovementType).HasColumnName("movement_type").HasMaxLength(10).IsRequired();
                entity.Property(m => m.Quantity).HasColumnName("quantity").IsRequired();
                entity.Property(m => m.Notes).HasColumnName("notes").HasMaxLength(500);
                entity.Property(m => m.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone");
                entity.HasOne(m => m.Product).WithMany().HasForeignKey(m => m.ProductId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

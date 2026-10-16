using RavingoTest.Models;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace RavingoTest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<StockItem> StockItems => Set<StockItem>();
        public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<StockItem>()
                .HasIndex(s => new { s.BranchId, s.ProductId })
                .IsUnique();

            
            modelBuilder.Entity<StockTransfer>()
                .HasOne(t => t.FromBranch)
                .WithMany()
                .HasForeignKey(t => t.FromBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockTransfer>()
                .HasOne(t => t.ToBranch)
                .WithMany()
                .HasForeignKey(t => t.ToBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockTransfer>()
                .HasOne(t => t.Product)
                .WithMany()
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

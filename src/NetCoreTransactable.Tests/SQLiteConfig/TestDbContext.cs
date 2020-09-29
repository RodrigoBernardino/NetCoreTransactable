using Microsoft.EntityFrameworkCore;
using NetCoreTransactable.Tests.Domain.Models;

namespace NetCoreTransactable.Tests.SQLiteConfig
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name);
                entity.Property(e => e.Type);
                entity.Property(e => e.Price);

                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name);
                entity.Property(e => e.Phone);

                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductId);
                entity.Property(e => e.ClientId);
                entity.Property(e => e.DateTime);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(entity => entity.ProductId);

                entity.HasOne(e => e.Client)
                    .WithMany()
                    .HasForeignKey(entity => entity.ClientId);
            });
        }
    }
}

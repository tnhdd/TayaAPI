using Microsoft.EntityFrameworkCore;
using TayaAPI.Models;

namespace TayaAPI.Data
{
    public class MovementDbContext(DbContextOptions<MovementDbContext> options) : DbContext(options)
    {
        public DbSet<Movement> Movements => Set<Movement>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique(); 
            });

            modelBuilder.Entity<Movement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OperationDate).IsRequired();
                entity.Property(e => e.ValueDate).IsRequired();
                entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);

                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict); 

                entity.HasIndex(e => e.CategoryId);
            });
        }
    }
}

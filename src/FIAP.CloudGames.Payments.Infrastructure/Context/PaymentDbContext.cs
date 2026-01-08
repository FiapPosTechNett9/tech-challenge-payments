using FIAP.CloudGames.Payments.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FIAP.CloudGames.Payments.Infrastructure.Context;

public class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
    : DbContext(options)
{
    public DbSet<Payment> Payments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(p => p.Status)
                .IsRequired();

            entity.Property(p => p.CreatedAt)
                .IsRequired();
        });
    }
}

using Microsoft.EntityFrameworkCore;
using Ponto.Domain.Entities;

namespace Ponto.Infra.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ClockEntity> Clocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClockEntity>()
            .HasOne(clock => clock.User)
            .WithMany(user => user.Clocks)
            .HasForeignKey(clock => clock.UserId);
    }
}
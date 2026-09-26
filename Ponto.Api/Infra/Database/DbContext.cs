using Microsoft.EntityFrameworkCore;
using Ponto.Api.Domain.Entities;

namespace Ponto.Api.Infra.Database;

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
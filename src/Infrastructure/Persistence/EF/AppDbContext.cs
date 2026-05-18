using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EF;

public sealed class AppDbContext : DbContext
{
    public DbSet<ConfigEntity> Configs => Set<ConfigEntity>();
    public DbSet<GameStateEntity> GameStates => Set<GameStateEntity>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigEntity>(entity =>
        {
            entity.HasKey(e => e.Name);
            entity.Property(e => e.Topology).HasConversion<string>();
            entity.Property(e => e.Player1TypeDifficulty).HasConversion<string?>();
            entity.Property(e => e.Player2TypeDifficulty).HasConversion<string?>();
        });

        modelBuilder.Entity<GameStateEntity>(entity =>
        {
            entity.HasKey(e => e.Name);
            entity.Property(e => e.ConfigTopology).HasConversion<string>();
            entity.Property(e => e.ConfigPlayer1TypeDifficulty).HasConversion<string?>();
            entity.Property(e => e.ConfigPlayer2TypeDifficulty).HasConversion<string?>();
        });
    }
}

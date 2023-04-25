using Chess.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace Chess.Infrastructure;

public class MatchDbContext: DbContext
{
    public DbSet<Match>? Matches { get; set; }
    public DbSet<MatchEvent>? Events { get; set; }

    public MatchDbContext() : base() { }
    public MatchDbContext(DbContextOptions<MatchDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
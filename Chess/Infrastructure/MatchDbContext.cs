using Chess.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace Chess.Infrastructure;

public class MatchDbContext: DbContext
{
    public DbSet<MatchEvent>? Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
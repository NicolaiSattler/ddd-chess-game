using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Chess.Infrastructure.Factory;

public class MatchSqliteDbContextFactory : IDesignTimeDbContextFactory<MatchDbContext>
{
    private const string ChessDbName = "ChessDb";
    public MatchDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationManager().SetBasePath(Directory.GetCurrentDirectory())
                                               .AddJsonFile("./infra-appsettings.json")
                                               .Build();

        var connectionString = config.GetConnectionString(ChessDbName);
        var optionsBuilder = new DbContextOptionsBuilder<MatchDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return new(optionsBuilder.Options);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chess.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ChessDatabaseName = "ChessDb";

    public static ServiceCollection AddInfrastructure(this ServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<MatchDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString(ChessDatabaseName);
            options.UseSqlite(connectionString);
        });
        return serviceCollection;
    }
}
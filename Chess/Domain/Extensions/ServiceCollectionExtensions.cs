using Chess.Domain.Aggregates;
using Microsoft.Extensions.DependencyInjection;

namespace Chess.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static ServiceCollection AddDomain(this ServiceCollection collection)
    {
        collection.AddScoped<IMatch, Match>();

        return collection;
    }
}
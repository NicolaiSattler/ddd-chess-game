using Chess.Application.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Chess.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<IApplicationService, ApplicationService>();
        collection.AddHostedService<TurnTimer>();
        return collection;
    }
}
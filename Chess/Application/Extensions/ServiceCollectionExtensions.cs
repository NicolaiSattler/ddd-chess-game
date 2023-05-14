using Chess.Application.Models;
using Chess.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chess.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.AddOptions<MatchOptions>()
                  .Bind(configuration.GetSection(MatchOptions.SectionName))
                  .ValidateOnStart();

        collection.AddScoped<IApplicationService, ApplicationService>();
        //TODO: How to make turntimer injectable?
        //collection.AddHostedService<TurnTimer>();
        collection.AddSingleton<TurnTimer>();
        collection.AddSingleton<ITurnTimer>(c => c.GetRequiredService<TurnTimer>());

        return collection;
    }
}
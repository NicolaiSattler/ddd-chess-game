using Chess.Application.Services;
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
        collection.AddSingleton<TimerService>();
        collection.AddSingleton<ITimerService>(c => c.GetRequiredService<TimerService>());
        collection.AddScoped<ITurnTimerInfoService, TurnTimerInfoService>();

        return collection;
    }
}
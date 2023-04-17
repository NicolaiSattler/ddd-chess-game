using Microsoft.Extensions.DependencyInjection;

namespace Chess.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static ServiceCollection AddApplication(this ServiceCollection collection)
    {
        collection.AddScoped<IApplicationService, ApplicationService>();
        return collection;
    }
}
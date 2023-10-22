namespace Chess.PlayerApi.Extensions;

public static class ServiceCollectionExtensions
{
    private const string DefaultScheme = "Bearer";

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection collection)
    {
        collection.AddAuthentication(DefaultScheme)
                  .AddJwtBearer(DefaultScheme, options =>
                  {
                      //TODO: Should be configured.
                      options.Authority = "https://localhost:5001";
                      options.TokenValidationParameters = new()
                      {
                          // Should be omitted unless ApiRource is used instead.
                          // See more info:
                          // https://docs.duendesoftware.com/identityserver/v6/apis/aspnetcore/jwt/#adding-audience-validation
                          ValidateAudience = false
                      };
                  });

        return collection;
    }

    public static IServiceCollection ConfigureAuthorization(this IServiceCollection collection)
    {
        collection.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "PlayerApi");
            });
        });
        return collection;
    }
}
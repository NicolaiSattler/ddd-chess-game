using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Chess.PlayerApi.Extensions;

public static class WebApplicationExtentions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", (ClaimsPrincipal user) => Results.Json(from c in user.Claims select new { c.Type, c.Value }))
           .RequireAuthorization("ApiScope");

        return app;
    }
}
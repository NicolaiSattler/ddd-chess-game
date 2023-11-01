using Chess.Application.Extensions;
using Chess.Infrastructure.Extensions;
using Chess.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddWebApp();

var app = builder.Build();

app.ConfigureMiddleware();

app.MapBlazorHub();
app.MapHub<MatchHub>(MatchHub.HubUrl);

app.MapFallbackToPage("/_Host");

app.Run();
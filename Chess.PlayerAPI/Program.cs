using Chess.PlayerApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();

using Chess.Application.Models;
using Chess.Web.Model;
using Chess.Web.Extensions;
using Microsoft.AspNetCore.Components;

namespace Chess.Web.Pages.Match.Setup;

public partial class SetupPage: ComponentBase
{
    private const string BoardUri = "/match/board/{0}";

    [Inject]
    private IApplicationService? ApplicationService { get; set; }
    [Inject]
    private ILogger<SetupPage>? Logger { get; set; }
    [Inject]
    private NavigationManager? NavigationManager { get; set;}
    private SetupModel Setup { get; set; } = new();

    private async Task NavigateToBoard()
    {
        var aggregateId = await StartMatchAsync();

        NavigationManager?.NavigateTo(string.Format(BoardUri, aggregateId));
    }

    private async Task<Guid> StartMatchAsync()
    {
        try
        {
            if (ApplicationService == null) return Guid.Empty;

            var command = Setup.CreateCommand();

            await ApplicationService.StartMatchAsync(command);

            return command.AggregateId;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Could not start a match for Member {MemberOne} and Member {MemberTwo}", Setup.MemberOne, Setup.MemberTwo);

            return Guid.Empty;
        }
    }
}
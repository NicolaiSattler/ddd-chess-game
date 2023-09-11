using Chess.Application.Models;
using Chess.Web.Model;
using Chess.Web.Extensions;
using Microsoft.AspNetCore.Components;
using Chess.Web.Validation;
using MudBlazor;

namespace Chess.Web.Pages.Match.Setup;

public partial class SetupPage: ComponentBase
{
    private const string BoardUri = "/match/board/{0}";
    private const string OverviewUri = "match/overview";

    [Inject]
    private IApplicationService? ApplicationService { get; set; }
    [Inject]
    private ILogger<SetupPage>? Logger { get; set; }
    [Inject]
    private NavigationManager? NavigationManager { get; set;}
    [Inject]
    private SetupModelValidator Validator { get; set; } = new();

    private SetupModel Setup { get; set; } = new();
    private MudForm? _form;

    private async Task NavigateToBoard()
    {
        await _form!.Validate();

        if (_form != null && !_form.IsValid) return;

        //If Id's are not parsed correctly, an error occurres.
        var aggregateId = await StartMatchAsync();

        NavigationManager?.NavigateTo(string.Format(BoardUri, aggregateId), true);
    }

    private void Back() => NavigationManager?.NavigateTo(OverviewUri);

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
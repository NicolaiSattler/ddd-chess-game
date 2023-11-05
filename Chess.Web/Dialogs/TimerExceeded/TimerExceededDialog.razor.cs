using Chess.Domain.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;

namespace Chess.Web.Dialogs.TimerExceeded;

public partial class TimerExceededDialog: ComponentBase
{
    public const string TimerExceededMessage = "Turn time of {0} has expired.";

    [CascadingParameter]
    private MudDialogInstance? MudDialog { get; set; }
    [Inject]
    private IOptions<MatchOptions>? MatchOptions { get; set; }
    public string Message => string.Format(TimerExceededMessage, MatchOptions?.Value.MaxTurnTime);

    private void Submit() => MudDialog?.Close(DialogResult.Ok(true));
}
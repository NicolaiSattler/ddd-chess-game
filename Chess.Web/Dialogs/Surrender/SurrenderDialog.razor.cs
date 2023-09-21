
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Chess.Web.Dialogs.Surrender;

public partial class SurrenderDialog
{
    private const string SurrenderDialogQuestion = "Are you sure you want to surrender?";

    [CascadingParameter]
    private MudDialogInstance? MudDialog { get; set; }

    private void Submit() => MudDialog?.Close(DialogResult.Ok(true));
    private void Cancel() => MudDialog?.Close(DialogResult.Cancel());
}
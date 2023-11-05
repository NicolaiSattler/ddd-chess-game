using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Chess.Web.Dialogs.Draw;

public partial class DrawDialog: ComponentBase
{
    public const string ConfirmDrawDialogQuestion = "Are you sure you want to propose a Draw?";
    public const string DrawDialogQuestion = "Your opponent is proposing a draw. Will you accept?";

    [CascadingParameter] private MudDialogInstance? MudDialog { get; set; }
    [Parameter] public string? ContentText { get; set;}

    private void Submit() => MudDialog?.Close(DialogResult.Ok(true));
    private void Cancel() => MudDialog?.Close(DialogResult.Cancel());

}
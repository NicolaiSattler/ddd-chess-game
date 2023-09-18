using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Chess.Web.Dialogs.Promotion;

public partial class PromotionDialog
{
    private const string PromotionDialogQustion = "To which piece do you want to promote your pawn?";

    [CascadingParameter]
    private MudDialogInstance? MudDialog { get; set; }

    private void Submit(PieceType type) => MudDialog?.Close(DialogResult.Ok(type));
}
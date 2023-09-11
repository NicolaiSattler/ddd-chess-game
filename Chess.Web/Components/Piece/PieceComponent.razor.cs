using Chess.Web.Components.Board;
using Microsoft.AspNetCore.Components;

namespace Chess.Web.Components.Piece;

public partial class PieceComponent
{
    [Parameter, EditorRequired]
    public Guid PieceId { get; set; }

    [Parameter, EditorRequired]
    public PieceType Type { get; set; }

    [Parameter, EditorRequired]
    public Color Color { get; set; }

    public string? HtmlClasses { get; private set; }

    [CascadingParameter]
    private BoardComponent? Parent { get; set;}

    private string GetPieceClass() => Type switch
    {
        PieceType.King => "king",
        PieceType.Queen => "queen",
        PieceType.Knight => "knight",
        PieceType.Bishop => "bishop",
        PieceType.Rook => "rook",
        PieceType.Pawn => "pawn",
        _ => throw new IndexOutOfRangeException(Type.ToString())
    };

    private async Task HandleDragStarted()
    {
        if (Parent != null && Parent.ActiveColor == Color)
        {
            await Parent.ShowAvailableMovesAsync(PieceId);

            Parent.ActivePieceId = PieceId;
        }
    }

    private void HandleDragEnded() => Parent?.HideAvailableMoves();

    protected override void OnParametersSet()
    {
        var color = Color == Color.White ? "white" : "black";
        var type = GetPieceClass();

        HtmlClasses = $"{color}-{type} piece";
    }
}
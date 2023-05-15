using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;
using Chess.Web.Pages.Test;
using Microsoft.AspNetCore.Components;

using Board = Chess.Web.Pages.Match.BoardPage;

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
    private Board? Parent { get; set;}

    private void HandleDragStarted(Guid pieceId)
    {
        Parent?.ShowAvailableMoves(pieceId);
    }
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

    private void HandleDragStart(Guid pieceId)
    {
        if (Parent != null)
            Parent.ActivePieceId = pieceId;
    }

    protected override void OnParametersSet()
    {
        var color = Color == Color.White ? "white" : "black";
        var type = GetPieceClass();

        HtmlClasses = $"{color}-{type} piece";
    }
}
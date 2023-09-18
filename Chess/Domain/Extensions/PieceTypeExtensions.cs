using Chess.Domain.Entities.Pieces;

namespace Chess.Domain.Extensions;

public static class PieceTypeExtensions
{
    public static char GetPieceNotation(this PieceType pieceType) => pieceType switch
    {
        PieceType.King => 'K',
        PieceType.Queen => 'Q',
        PieceType.Rook => 'R',
        PieceType.Bishop => 'B',
        PieceType.Knight => 'N',
        _ => throw new IndexOutOfRangeException("Unknown PieceType")
    };
}
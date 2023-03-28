using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;

namespace Chess.Domain.Utilities;

public static class Notation
{

    //TODO:
    //Castling
    //  0-0: kingside castle
    //  0-0-0: queenside castle
    //
    //Ambiguous origin both rooks are in same file/rank
    //
    //x: captures
    //+: check
    //#: checkmate

    public static string TurnNotation(Turn turn, IEnumerable<Piece> pieces)
    {
        Guard.Against.Null<Turn>(turn, nameof(turn));
        Guard.Against.Null<IEnumerable<Piece>>(pieces, nameof(pieces));

        var movingPiece = pieces.Single(p => p.Position == turn.StartPosition);
        var hasCapturedPiece = pieces.Any(p => p.Position == turn.EndPosition);
        var pieceNotation = GetPieceNotation(movingPiece, hasCapturedPiece);

        return hasCapturedPiece
            ? $"{pieceNotation}x{turn.EndPosition?.ToString()}"
            : $"{pieceNotation}{turn.EndPosition?.ToString()}";
    }

    private static string GetPieceNotation(Piece piece, bool isCaptured) => piece.Type switch
    {
        PieceType.King => "K",
        PieceType.Queen => "Q",
        PieceType.Rook => "R",
        PieceType.Bishop => "B",
        PieceType.Knight => "N",
        PieceType.Pawn when isCaptured => piece.Position.File.ToString(),
        _ => throw new IndexOutOfRangeException("Unknown PieceType")
    };
}

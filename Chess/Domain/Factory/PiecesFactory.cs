using System.Collections.Generic;
using Chess.Core.Match.Entities;
using Chess.Core.Match.Entities.Pieces;
using Chess.Core.Match.ValueObjects;

namespace Chess.Core.Match.Factory;

public class PiecesFactory
{
    public static Dictionary<int, PieceType> StartPositions = new Dictionary<int, PieceType>
    {
        { 1, PieceType.Rook },
        { 2, PieceType.Knight },
        { 3, PieceType.Bishop },
        { 4, PieceType.Queen },
        { 5, PieceType.King },
        { 6, PieceType.Bishop },
        { 7, PieceType.Knight },
        { 8, PieceType.Rook },
    };

    public static Piece CreatePiece(PieceType type, Square position, Guid id) => type switch
    {
        PieceType.Rook => new Rook(id) { Position = position},
        PieceType.Knight => new Knight(id) { Position = position},
        PieceType.Bishop => new Bishop(id) { Position = position},
        PieceType.Queen => new Queen(id) { Position = position},
        PieceType.King => new King(id) { Position = position},
        PieceType.Pawn => new Pawn(id) { Position = position},
        _ => throw new InvalidOperationException($"{type.ToString()} is not a valid type.")
    };

    public static IEnumerable<Piece> CreatePiecesForColor(Color color)
    {
        var startRow = color == Color.Black ? 8 : 1;
        var pawnRow = color == Color.Black ? 7 : 2;
        var result = new List<Piece>();

        for (int i = 1; i < 9; i++)
        {
            var pieceType = StartPositions[i];

            result.Add(CreatePiece(PieceType.Pawn, new Square(i, pawnRow), Guid.NewGuid()));
            result.Add(CreatePiece(pieceType, new Square(i, startRow), Guid.NewGuid()));
        }

        return result;
    }
}
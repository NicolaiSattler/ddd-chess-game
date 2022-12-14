using System.Collections.Generic;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

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

    public static Piece CreatePiece(PieceType? type, Square? position, Guid id, Color color) => type switch
    {
        PieceType.Rook => new Rook() { Position = position, Color = color },
        PieceType.Knight => new Knight() { Position = position, Color = color },
        PieceType.Bishop => new Bishop() { Position = position, Color = color },
        PieceType.Queen => new Queen() { Position = position, Color = color },
        PieceType.King => new King() { Position = position, Color = color },
        PieceType.Pawn => new Pawn() { Position = position, Color = color },
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
            result.Add(CreatePiece(PieceType.Pawn, new Square((File)i, pawnRow), Guid.NewGuid(), color));
            result.Add(CreatePiece(pieceType, new Square((File)i, startRow), Guid.NewGuid(), color));
        }

        return result;
    }
}
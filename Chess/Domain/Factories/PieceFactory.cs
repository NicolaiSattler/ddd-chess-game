using System.Collections.Generic;

using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Factories;

public static class PieceFactory
{
    private static Dictionary<int, PieceType> GetStartPositions() => new()
    {
        { 1, PieceType.Rook },
        { 2, PieceType.Knight },
        { 3, PieceType.Bishop },
        { 6, PieceType.Bishop },
        { 7, PieceType.Knight },
        { 8, PieceType.Rook },
    };

    public static Piece CreatePiece(PieceType type, Square position, Guid id, Color color)
    {
        Piece piece = type switch
        {
            PieceType.Rook => new Rook(id) { Color = color },
            PieceType.Knight => new Knight(id) { Color = color },
            PieceType.Bishop => new Bishop(id) { Color = color },
            PieceType.Queen => new Queen(id) { Color = color },
            PieceType.King => new King() { Color = color },
            PieceType.Pawn => new Pawn() { Color = color },
            _ => throw new InvalidOperationException($"{type} is not a valid type.")
        };
        piece.MoveTo(position);

        return piece;
    }

    public static Piece CreatePiece(PieceType type, Square position, Color color)
    {
        Piece piece = type switch
        {
            PieceType.Rook => new Rook() { Color = color },
            PieceType.Knight => new Knight() { Color = color },
            PieceType.Bishop => new Bishop() { Color = color },
            PieceType.Queen => new Queen() { Color = color },
            PieceType.King => new King() { Color = color },
            PieceType.Pawn => new Pawn() { Color = color },
            _ => throw new InvalidOperationException($"{type} is not a valid type.")
        };
        piece.MoveTo(position);

        return piece;
    }

    public static IEnumerable<Piece> CreatePiecesForColor(Color color)
    {
        var startRow = color == Color.Black ? 8 : 1;
        var pawnRow = color == Color.Black ? 7 : 2;
        var result = new List<Piece>();
        var startPositions = GetStartPositions();

        for (int i = 1; i < 9; i++)
        {
            result.Add(CreatePiece(PieceType.Pawn, new Square((File)i, pawnRow), color));

            if (startPositions.ContainsKey(i))
            {
                var pieceType = startPositions[i];
                result.Add(CreatePiece(pieceType, new Square((File)i, startRow), color));
            }
        }

        result.Add(CreatePiece(PieceType.Queen, new Square(File.D, startRow), color));
        result.Add(CreatePiece(PieceType.King, new Square(File.E, startRow), color));

        return result;
    }
}
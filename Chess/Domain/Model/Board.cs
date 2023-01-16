using System.Collections.Generic;
using System.Linq;
using Chess.Core.Match.Events;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Model;

public class Board
{
    public static bool PieceIsCaptured(TurnTaken @event, IEnumerable<Piece>? pieces)
    {
        var movingPiece = pieces?.FirstOrDefault(p => p.Position == @event.StartPosition)
                        ?? throw new InvalidOperationException("Piece does not exists!");

        return pieces?.Any(p => p.Color != movingPiece.Color && p.Position == @event.EndPosition) ?? false;
    }

    public static bool PawnIsPromoted(TurnTaken? @event, IEnumerable<Piece>? pieces)
    {
        var movingPiece = pieces?.FirstOrDefault(p => p.Position == @event?.StartPosition)
                ?? throw new InvalidOperationException("Piece cannot be found.");

        if (movingPiece is Pawn pawn)
        {
            var promotionRank = Color.White == pawn.Color ? 8 : 1;
            return @event?.EndPosition?.Rank == promotionRank;
        }

        return false;
    }

    //TODO: Unit test.
    public static bool DirectionIsObstructed(IEnumerable<Piece> pieces, Square start, Square end)
        => DirectionIsObstructed(pieces, GetMoveDirection(start, end), start, end);

    //TODO: Unit test
    public static bool IsCheck(King king, IEnumerable<Piece> pieces)
    {
        var isUnReachable = KingIsUnreachable(king, pieces);

        if (isUnReachable) return false;

        var pieceCanReachKing = CanPieceReachKing(king, pieces);

        if (pieceCanReachKing) return true;

        return false;
    }

    private static bool CanPieceReachKing(King king, IEnumerable<Piece> pieces)
    {
        _ = king ?? throw new ArgumentNullException(nameof(king));
        _ = pieces ?? throw new ArgumentNullException(nameof(pieces));

        if (king.Position == null) throw new InvalidOperationException("King doesn't have a position.");

        var opponentPieces = pieces.Where(p => p.Color != king?.Color);

        foreach (var piece in opponentPieces)
        {
            if (piece.Position == null) throw new InvalidOperationException("Piece doesn't have a position.");

            var availableMoves = piece.GetAttackRange();

            if (availableMoves.Any(s => s == king?.Position))
            {
                var pieceIsObstructed = Board.DirectionIsObstructed(pieces, piece.Position, king.Position);

                if (!pieceIsObstructed)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool KingIsUnreachable(King? king, IEnumerable<Piece>? pieces)
                        => king?.GetAttackRange()?.All(s => pieces?.Any(p => p.Position == s) ?? false) ?? false;

    private static DirectionType GetMoveDirection(Square? start, Square? end)
    {
        _ = start ?? throw new ArgumentNullException(nameof(start));
        _ = end ?? throw new InvalidOperationException(nameof(end));

        var result = DirectionType.Undefined;

        if (start.File == end.File)
            return GetVerticalDirection(result, start, end);

        if (start.Rank == end.Rank)
            return GetHorizontalDirection(result, start, end);

        result = GetVerticalDirection(result, start, end);
        result = GetHorizontalDirection(result, start, end);

        return result;
    }
    private static bool DirectionIsObstructed(IEnumerable<Piece> pieces, DirectionType direction, Square start, Square end) => (direction) switch
    {
        DirectionType.Left => pieces.Any(p => (int?)p.Position?.File < (int)start.File && (int?)p.Position?.File > (int)end.File),
        DirectionType.Right => pieces.Any(p => (int?)p.Position?.File > (int)start.File && (int?)p.Position?.File < (int)end.File),
        DirectionType.Up => pieces.Any(p => p.Position?.Rank > start.Rank && p.Position?.Rank < end.Rank),
        DirectionType.Down => pieces.Any(p => p.Position?.Rank < start.Rank && p.Position?.Rank > end.Rank),
        DirectionType.Left | DirectionType.Up => ValidateDiagonalLeftUpObstruction(pieces, start, end),
        DirectionType.Right | DirectionType.Up => ValidateDiagonalRightUpObstruction(pieces, start, end),
        DirectionType.Left | DirectionType.Down => ValidateDiagonalLeftDownObstruction(pieces, start, end),
        DirectionType.Right | DirectionType.Down => ValidateDiagonalRightDownObstruction(pieces, start, end),
        _ => throw new InvalidOperationException("Oeps")
    };

    private static bool ValidateDiagonalRightDownObstruction(IEnumerable<Piece> pieces, Square start, Square end)
    {
        for (var x = start.Rank; x > end.Rank; x--)
            for (var y = (int)start.File; y < (int)end.File; y++)
                if (pieces.Any(p => p.Position == new Square((File)y, x)))
                    return true;
        return false;
    }

    private static bool ValidateDiagonalLeftDownObstruction(IEnumerable<Piece> pieces, Square start, Square end)
    {
        for (var x = end.Rank; x > start.Rank; x--)
            for (var y = (int)start.File; y > (int)end.File; y--)
                if (pieces.Any(p => p.Position == new Square((File)y, x)))
                    return true;
        return false;
    }

    private static bool ValidateDiagonalRightUpObstruction(IEnumerable<Piece> pieces, Square start, Square end)
    {
        for (var x = start.Rank; x < end.Rank; x++)
            for (var y = (int)start.File; y < (int)end.File; y++)
                if (pieces.Any(p => p.Position == new Square((File)y, x)))
                    return true;
        return false;
    }

    private static bool ValidateDiagonalLeftUpObstruction(IEnumerable<Piece> pieces, Square start, Square end)
    {
        for (var x = start.Rank; x < end.Rank; x++)
            for (var y = (int)start.File; y > (int)end.File; y--)
                if (pieces.Any(p => p.Position == new Square((File)y, x)))
                    return true;
        return false;
    }

    private static DirectionType GetVerticalDirection(DirectionType type, Square start, Square end) => start?.Rank < end?.Rank
                                            ? type |= DirectionType.Up
                                            : type |= DirectionType.Down;

    private static DirectionType GetHorizontalDirection(DirectionType type, Square start, Square end) => start?.File < end?.File
                                            ? type |=  DirectionType.Right
                                            : type |= DirectionType.Left;
}
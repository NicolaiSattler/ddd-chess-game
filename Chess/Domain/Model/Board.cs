using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core.Match.Events;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Model;

public class Board
{
    public static bool PieceIsCaptured(TurnTaken? @event, IEnumerable<Piece>? pieces)
    {
        var movingPiece = pieces?.FirstOrDefault(p => p.Position == @event?.StartPosition)
                        ?? throw new InvalidOperationException("Piece does not exists!");

        return pieces?.Any(p => p.Color != movingPiece.Color && p.Position == @event?.EndPosition) ?? false;
    }

    public static bool? DirectionIsObstructed(IEnumerable<Piece>? pieces, Square? start, Square? end)
        => DirectionIsObstructed(pieces, GetMoveDirection(start, end), start, end);

    public static bool IsCheck(King? king, IEnumerable<Piece>? pieces)
    {
        Guard.Against.Null<King?>(king, nameof(King));
        Guard.Against.Null<IEnumerable<Piece>?>(pieces, nameof(pieces));

        var isUnReachable = KingIsUnreachable(king, pieces);

        if (isUnReachable) return false;

        var pieceCanReachKing = PieceCanReachKing(king, pieces);

        if (pieceCanReachKing) return true;

        return false;
    }

    public static bool IsCheckMate(King? king, IEnumerable<Piece>? pieces)
    {
        Guard.Against.Null<King?>(king, nameof(King));
        Guard.Against.Null<IEnumerable<Piece>?>(pieces, nameof(pieces));

        var isCheck = IsCheck(king, pieces);

        if (isCheck)
        {
            var attackRange = king?.GetAttackRange();
            var opponentPieces = pieces?.Where(p => p.Color != king?.Color);
            return KingCannotEscapeCheck(attackRange, pieces, opponentPieces);
        }

        return false;
    }

    public static bool IsStalemate(Color color, IEnumerable<Piece> pieces)
    {
        //Check if all pieces are blocked and/or if the king has movement options.
        throw new NotImplementedException();

    }

    private static bool PieceCanReachKing(King king, IEnumerable<Piece> pieces)
    {
        Guard.Against.Null<King>(king, nameof(king));
        Guard.Against.Null<IEnumerable<Piece>>(pieces, nameof(pieces));
        Guard.Against.InvalidInput<King>(king, nameof(king), k => k.Position != null, "King doesn't have a position.");

        var opponentPieces = pieces.Where(p => p.Color != king?.Color);
        return PositionIsReachableByPiece(king?.Position, pieces, opponentPieces) != null;
    }

    private static bool KingCannotEscapeCheck(IEnumerable<Square>? attackRange, IEnumerable<Piece>? allPieces, IEnumerable<Piece>? opponentPieces)
    {
        Guard.Against.NullOrEmpty(attackRange, nameof(attackRange));
        Guard.Against.NullOrEmpty(allPieces, nameof(allPieces));
        Guard.Against.NullOrEmpty(opponentPieces, nameof(opponentPieces));

        var attackingPieces = attackRange.Select(square => PositionIsReachableByPiece(square, allPieces, opponentPieces))
                                         ?.Where(p => p != null);
        var count = attackingPieces?.Count() ?? 0;

        if (attackingPieces != null && count > 0)
        {
            if (count == 1)
            {
                var opponent = attackingPieces.First();
                var alliedPieces = allPieces.Where(p => p.Color != opponent?.Color);
                var opponentCanBeAttacked = PositionIsReachableByPiece(opponent?.Position, alliedPieces, alliedPieces) != null;

                return !opponentCanBeAttacked;
            }

            return true;
        }

        return false;
    }

    private static Piece? PositionIsReachableByPiece(Square? position, IEnumerable<Piece> pieces, IEnumerable<Piece> opponentPieces)
    {
        foreach (var piece in opponentPieces)
        {
            Guard.Against.InvalidInput<Piece>(piece, nameof(piece), k => k.Position != null, "Piece doesn't have a position.");

            var availableMoves = piece.GetAttackRange();

            if (availableMoves.Any(s => s == position))
            {
                var pieceIsObstructed = Board.DirectionIsObstructed(pieces, piece.Position, position) ?? false;

                if (!pieceIsObstructed || piece.Type == PieceType.Knight)
                {
                    return piece;
                }
            }
        }

        return null;
    }

    private static bool KingIsUnreachable(King? king, IEnumerable<Piece>? pieces)
                        => king?.GetAttackRange()?.All(s => pieces?.Any(p => p.Position == s) ?? false) ?? false;

    private static DirectionType GetMoveDirection(Square? start, Square? end)
    {
        Guard.Against.Null<Square?>(start, nameof(start));
        Guard.Against.Null<Square?>(end, nameof(end));

        var result = DirectionType.Undefined;

        if (start.File == end.File)
            return GetVerticalDirection(result, start, end);

        if (start.Rank == end.Rank)
            return GetHorizontalDirection(result, start, end);

        result = GetVerticalDirection(result, start, end);
        result = GetHorizontalDirection(result, start, end);

        return result;
    }
    private static bool? DirectionIsObstructed(IEnumerable<Piece>? pieces, DirectionType direction, Square? start, Square? end) => (direction) switch
    {
        DirectionType.Left => pieces?.Any(p => (int?)p.Position?.File < (int?)start?.File && (int?)p.Position?.File > (int?)end?.File),
        DirectionType.Right => pieces?.Any(p => (int?)p.Position?.File > (int?)start?.File && (int?)p.Position?.File < (int?)end?.File),
        DirectionType.Up => pieces?.Any(p => p.Position?.Rank > start?.Rank && p.Position?.Rank < end?.Rank),
        DirectionType.Down => pieces?.Any(p => p.Position?.Rank < start?.Rank && p.Position?.Rank > end?.Rank),
        DirectionType.Left | DirectionType.Up => ValidateDiagonalLeftUpObstruction(pieces, start, end),
        DirectionType.Right | DirectionType.Up => ValidateDiagonalRightUpObstruction(pieces, start, end),
        DirectionType.Left | DirectionType.Down => ValidateDiagonalLeftDownObstruction(pieces, start, end),
        DirectionType.Right | DirectionType.Down => ValidateDiagonalRightDownObstruction(pieces, start, end),
        _ => throw new InvalidOperationException("Oeps")
    };

    private static bool ValidateDiagonalRightDownObstruction(IEnumerable<Piece>? pieces, Square? start, Square? end)
    {
        for (var x = start?.Rank - 1; x > end?.Rank; x--)
            for (var y = (int?)start?.File - 1; y < (int?)end?.File; y++)
                if (pieces?.Any(p => p.Position == new Square((File)y, x)) ?? false)
                    return true;
        return false;
    }

    private static bool ValidateDiagonalLeftDownObstruction(IEnumerable<Piece>? pieces, Square? start, Square? end)
    {
        for (var x = end?.Rank - 1; x > start?.Rank; x--)
            for (var y = (int?)start?.File - 1; y > (int?)end?.File; y--)
                if (pieces?.Any(p => p.Position == new Square((File)y, x)) ?? false)
                    return true;
        return false;
    }

    private static bool ValidateDiagonalRightUpObstruction(IEnumerable<Piece>? pieces, Square? start, Square? end)
    {
        for (var x = start?.Rank + 1; x < end?.Rank; x++)
            for (var y = (int?)start?.File + 1; y < (int?)end?.File; y++)
                if (pieces?.Any(p => p.Position == new Square((File)y, x)) ?? false)
                    return true;
        return false;
    }

    private static bool ValidateDiagonalLeftUpObstruction(IEnumerable<Piece>? pieces, Square? start, Square? end)
    {
        for (var x = start?.Rank + 1; x < end?.Rank; x++)
            for (var y = (int?)start?.File + 1; y > (int?)end?.File; y--)
                if (pieces?.Any(p => p.Position == new Square((File)y, x)) ?? false)
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
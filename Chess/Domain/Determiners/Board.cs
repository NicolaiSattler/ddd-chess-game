using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core.Match.Events;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Determiners;

public class Board
{
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

    //TODO: Unit Tests
    public static bool IsStalemate(Color? color, IEnumerable<Piece>? pieces)
    {
        var king = pieces?.FirstOrDefault(p => p.Color == color && p.Type == PieceType.King);
        var atTurnPieces = pieces?.Where(p => p.Color == color) ?? Enumerable.Empty<Piece>();
        var opponentPieces = pieces?.Where(p => p.Color != color) ?? Enumerable.Empty<Piece>();

        var pieceHasNoValidMoves = true;
        var kingHasNoValidMoves = king?.GetAttackRange()
                                      ?.Where(p => !pieces?.Any(q => q.Position == p) ?? false)
                                      ?.All(p => PositionIsReachableByPiece(p, pieces, opponentPieces) != null) ?? false;

        foreach (var piece in atTurnPieces.Where(p => p.Type != PieceType.King))
        {
            var availableMoves = piece.Type == PieceType.Pawn ? GetAvailablePawnMoves(pieces, piece) : piece.GetAttackRange();
            var allMovesObstructed = availableMoves.All(p => DirectionIsObstructed(pieces, piece.Position, p) == true);

            if (!allMovesObstructed)
            {
                pieceHasNoValidMoves = false;
                break;
            }
        }

        return pieceHasNoValidMoves && kingHasNoValidMoves;
    }

    /// <summary>
    /// Retrieve all valid pawn movements.
    /// </summary>
    private static IEnumerable<Square> GetAvailablePawnMoves(IEnumerable<Piece>? pieces, Piece piece)
    {
        var opponentPieces = pieces?.Where(p => p.Color != piece.Color);
        return piece.GetAttackRange()
                    .Where(p => (p.File != piece.Position?.File && (opponentPieces?.All(q => q.Position == p) ?? false))
                                || p.File == piece.Position?.File);
    }

    public static Piece? PositionIsReachableByPiece(Square? position, IEnumerable<Piece>? pieces, IEnumerable<Piece>? opponentPieces)
    {
        Guard.Against.Null<IEnumerable<Piece>?>(opponentPieces);

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

    public static bool PieceIsCaptured(TurnTaken? @event, IEnumerable<Piece>? pieces)
    {
        var movingPiece = pieces?.FirstOrDefault(p => p.Position == @event?.StartPosition)
                        ?? throw new InvalidOperationException("Piece does not exists!");

        return pieces?.Any(p => p.Color != movingPiece.Color && p.Position == @event?.EndPosition) ?? false;
    }

    public static bool? DirectionIsObstructed(IEnumerable<Piece>? pieces, Square? start, Square? end)
    {
        var piece = pieces?.FirstOrDefault(p => p.Position == start && p.Type == PieceType.Pawn);

        return piece is Pawn ? DirectionIsObstructedForPawn(pieces, piece, end)
                             : DirectionIsObstructed(pieces, GetMoveDirection(start, end), start, end);
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

    private static Func<Square?, Square?, bool> IsSameFile = (a, b) => a?.File == b?.File;
    private static Func<Square?, Square?, bool> IsSameRank = (a, b) => a?.Rank == b?.Rank;
    private static bool? DirectionIsObstructedForPawn(IEnumerable<Piece>? pieces, Piece? pawn, Square? end)
    {
        if (pieces?.Any(p => p.Position == end) ?? false) return true;

        var ranks = pawn?.Color == Color.White
                  ? end?.Rank - pawn?.Position?.Rank
                  : pawn?.Position?.Rank - end?.Rank;

        if (ranks > 1)
        {
            var passingRank = pawn?.Color == Color.White
                            ? pawn?.Position?.Rank + 1
                            : pawn?.Position?.Rank - 1;

            return pieces?.Any(p => p.Position?.Rank == passingRank && p.Position?.File == pawn?.Position?.File);
        }

        return false;
    }
    private static bool? DirectionIsObstructed(IEnumerable<Piece>? pieces, DirectionType direction, Square? start, Square? end) => (direction) switch
    {
        DirectionType.Left => pieces?.Any(p => (int?)p.Position?.File < (int?)start?.File && (int?)p.Position?.File > (int?)end?.File && IsSameRank(start, p.Position)),
        DirectionType.Right => pieces?.Any(p => (int?)p.Position?.File > (int?)start?.File && (int?)p.Position?.File < (int?)end?.File && IsSameRank(start, p.Position)),
        DirectionType.Up => pieces?.Any(p => p.Position?.Rank > start?.Rank && p.Position?.Rank < end?.Rank && IsSameFile(start, p.Position)),
        DirectionType.Down => pieces?.Any(p => p.Position?.Rank < start?.Rank && p.Position?.Rank > end?.Rank && IsSameFile(start, p.Position)),
        DirectionType.Left | DirectionType.Up => ValidateDiagonalLeftUpObstruction(pieces, start, end),
        DirectionType.Right | DirectionType.Up => ValidateDiagonalRightUpObstruction(pieces, start, end),
        DirectionType.Left | DirectionType.Down => ValidateDiagonalLeftDownObstruction(pieces, start, end),
        DirectionType.Right | DirectionType.Down => ValidateDiagonalRightDownObstruction(pieces, start, end),
        _ => throw new InvalidOperationException("Oeps")
    };

    private static bool ValidateDiagonalRightDownObstruction(IEnumerable<Piece>? pieces, Square? start, Square? end)
    {
        var y = (int?)start?.File + 1;

        for (var x = start?.Rank - 1; x > end?.Rank; x--)
        {
            if (pieces?.Any(p => p.Position == new Square((File)y, x)) ?? false)
            {
                return true;
            }

            if (y < (int?)end?.File)
            {
                y++;
            }
            else break;
        }

        return false;
    }

    private static bool ValidateDiagonalLeftDownObstruction(IEnumerable<Piece>? pieces, Square? start, Square? end)
    {
        var y = (int?)start?.File - 1;

        for (var x = end?.Rank - 1; x > start?.Rank; x--)
        {
            if (pieces?.Any(p => p.Position == new Square((File)y, x)) ?? false)
                return true;

            if (y > (int?)end?.File)
            {
                y--;
            }
            else break;
        }

        return false;
    }

    private static bool ValidateDiagonalRightUpObstruction(IEnumerable<Piece>? pieces, Square? start, Square? end)
    {
        var y = (int?)start?.File + 1;

        for (var x = start?.Rank + 1; x < end?.Rank; x++)
        {
            if (pieces?.Any(p => p.Position == new Square((File)y, x)) ?? false)
                return true;

            if (y < (int?)end?.File)
            {
                y++;
            }
            else break;
        }

        return false;
    }

    private static bool ValidateDiagonalLeftUpObstruction(IEnumerable<Piece>? pieces, Square? start, Square? end)
    {
        var y = (int?)start?.File + 1;

        for (var x = start?.Rank + 1; x < end?.Rank; x++)
        {
            if (pieces?.Any(p => p.Position == new Square((File)y, x)) ?? false)
                return true;

            if (y > (int?)end?.File)
            {
                y--;
            }
            else break;
        }

        return false;
    }

    private static DirectionType GetVerticalDirection(DirectionType type, Square start, Square end) => start?.Rank < end?.Rank
                                            ? type |= DirectionType.Up
                                            : type |= DirectionType.Down;

    private static DirectionType GetHorizontalDirection(DirectionType type, Square start, Square end) => start?.File < end?.File
                                            ? type |= DirectionType.Right
                                            : type |= DirectionType.Left;
}

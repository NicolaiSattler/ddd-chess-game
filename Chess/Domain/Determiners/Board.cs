using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Domain.Events;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Determiners;

public class Board
{
    private static readonly Func<Square, Square, bool> IsSameFile = (a, b) => a.File == b.File;
    private static readonly Func<Square, Square, bool> IsSameRank = (a, b) => a.Rank == b.Rank;

    public static bool IsCheck(King king, IEnumerable<Piece> pieces)
    {
        Guard.Against.Null(king, nameof(King));
        Guard.Against.Null(pieces, nameof(pieces));

        return !KingIsUnreachable(king, pieces) && GetPiecesThatReachKing(king, pieces).Any();
    }

    public static bool IsCheckMate(King king, IEnumerable<Piece> pieces)
    {
        Guard.Against.Null(king, nameof(King));
        Guard.Against.Null(pieces, nameof(pieces));

        var isCheck = IsCheck(king, pieces);

        if (isCheck)
        {
            var opponentPieces = pieces.Where(p => p.Color != king.Color);
            return KingCannotEscapeCheck(king, pieces, opponentPieces);
        }

        return false;
    }

    public static bool IsStalemate(Color color, IEnumerable<Piece> pieces)
    {
        Guard.Against.Null(pieces, nameof(pieces));

        var king = pieces.FirstOrDefault(p => p.Color == color && p.Type == PieceType.King)
                 ?? throw new InvalidOperationException("King cannot be found!");
        var atTurnPieces = pieces.Where(p => p.Color == color) ?? Enumerable.Empty<Piece>();
        var opponentPieces = pieces.Where(p => p.Color != color) ?? Enumerable.Empty<Piece>();
        var pieceHasNoValidMoves = true;
        var kingHasNoValidMoves = king.GetAttackRange()
                                      .Where(p => !pieces.Any(q => q.Position == p))
                                      .All(p => GetPiecesThatCanReachPosition(p, pieces, opponentPieces).Any());

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

    public static IEnumerable<Piece> GetPiecesThatCanReachPosition(Square position, IEnumerable<Piece> pieces, IEnumerable<Piece> opponentPieces)
    {
        Guard.Against.Null(position, nameof(position));
        Guard.Against.Null(pieces, nameof(pieces));
        Guard.Against.Null(opponentPieces, nameof(opponentPieces));

        var result = new List<Piece>();

        foreach (var piece in opponentPieces)
        {
            Guard.Against.InvalidInput(piece, nameof(piece), k => k.Position != null, "Piece doesn't have a position.");

            var availableMoves = piece is Pawn
                ? piece.GetAttackRange().Where(p => p.File != piece.Position.File)
                : piece.GetAttackRange();

            if (availableMoves.Any(s => s == position))
            {
                var pieceIsObstructed = DirectionIsObstructed(pieces, piece.Position, position);

                if (!pieceIsObstructed || piece.Type == PieceType.Knight)
                {
                    result.Add(piece);
                }
            }
        }

        return result;
    }

    public static bool PieceIsCaptured(TurnTaken @event, IEnumerable<Piece> pieces)
    {
        Guard.Against.Null(@event, nameof(@event));
        Guard.Against.Null(pieces, nameof(pieces));

        var movingPiece = pieces.FirstOrDefault(p => p.Position == @event.StartPosition)
                        ?? throw new InvalidOperationException("Piece does not exists!");

        return pieces.Any(p => p.Color != movingPiece.Color && p.Position == @event.EndPosition);
    }

    public static bool DirectionIsObstructed(IEnumerable<Piece> pieces, Square start, Square end)
    {
        Guard.Against.Null(pieces, nameof(pieces));
        Guard.Against.Null(start, nameof(start));
        Guard.Against.Null(end, nameof(end));

        var piece = pieces.FirstOrDefault(p => p.Position == start);
        var direction = GetMoveDirection(start, end);

        if (piece == null) return false;

        return piece switch
        {
            Pawn => DirectionIsObstructedForPawn(pieces, piece, end),
            Knight => DirectionIsObstructedForKnight(pieces, piece, end),
            _ => DirectionIsObstructed(pieces, direction, start, end)
                 || pieces.Any(p => p.Position == end && p.Color == piece.Color)
        };
    }

    public static IEnumerable<Piece> GetPiecesThatReachKing(King king, IEnumerable<Piece> pieces)
    {
        Guard.Against.Null(king, nameof(king));
        Guard.Against.Null(pieces, nameof(pieces));
        Guard.Against.InvalidInput(king, nameof(king), k => k.Position != null, "King doesn't have a position.");

        var opponentPieces = pieces.Where(p => p.Color != king.Color);
        return GetPiecesThatCanReachPosition(king.Position, pieces, opponentPieces);
    }

    private static bool KingCannotEscapeCheck(King king, IEnumerable<Piece> allPieces, IEnumerable<Piece> opponentPieces)
    {
        Guard.Against.Null(king, nameof(king));
        Guard.Against.NullOrEmpty(allPieces, nameof(allPieces));
        Guard.Against.NullOrEmpty(opponentPieces, nameof(opponentPieces));

        var attackingPieces = king.GetAttackRange()
                                  .SelectMany(square => GetPiecesThatCanReachPosition(square, allPieces, opponentPieces))
                                  .Distinct();

        if (!attackingPieces.Any())
        {
            return false;
        }
        else if (attackingPieces.Count() == 1)
        {
            var opponent = attackingPieces.First();
            var alliedPieces = allPieces.Where(p => p.Color != opponent.Color);
            var alliedPieceCanReachAttacker = GetPiecesThatCanReachPosition(opponent.Position, allPieces, alliedPieces).Any();

            return !alliedPieceCanReachAttacker && !AttackCanBeBlocked(opponent, king, allPieces);
        }

        return true;
    }

    private static bool AttackCanBeBlocked(Piece attackingPiece, Piece defender, IEnumerable<Piece> pieces)
    {
        Guard.Against.Null(attackingPiece, nameof(attackingPiece));
        Guard.Against.Null(defender, nameof(defender));

        var attackDirection = GetMoveDirection(attackingPiece.Position, defender.Position);
        var path = attackingPiece.GetAttackRange().Where(p => GetMoveDirection(p, defender.Position) == attackDirection);
        var defendingPieces = pieces.Where(p => p.Color == defender.Color);

        return path.Any(p => GetPiecesThatCanReachPosition(p, pieces, defendingPieces).Any());
    }

    /// <summary>
    /// Retrieve all valid pawn movements.
    /// </summary>
    private static IEnumerable<Square> GetAvailablePawnMoves(IEnumerable<Piece> pieces, Piece piece)
    {
        var opponentPieces = pieces.Where(p => p.Color != piece.Color);

        return piece.GetAttackRange()
                    .Where(p => (p.File != piece.Position.File && opponentPieces.All(q => q.Position == p))
                                || p.File == piece.Position.File);
    }

    private static bool KingIsUnreachable(King king, IEnumerable<Piece> pieces)
                        => king.GetAttackRange().All(s => pieces.Any(p => p.Position == s));

    private static DirectionType GetMoveDirection(Square start, Square end)
    {
        var result = DirectionType.Undefined;

        if (start.File == end.File)
            return GetVerticalDirection(result, start, end);

        if (start.Rank == end.Rank)
            return GetHorizontalDirection(result, start, end);

        result = GetVerticalDirection(result, start, end);
        result = GetHorizontalDirection(result, start, end);

        return result;
    }

    private static bool DirectionIsObstructedForPawn(IEnumerable<Piece> pieces, Piece pawn, Square end)
    {
        if (pieces.Any(p => p.Position == end && p.Color == pawn.Color)) return true;
        if (pieces.Any(p => p.Position == end) && pawn.Position.File == end.File) return true;

        var ranks = pawn.Color == Color.White
                  ? end.Rank - pawn.Position.Rank
                  : pawn.Position.Rank - end.Rank;

        if (ranks > 1)
        {
            var passingRank = pawn.Color == Color.White
                            ? pawn.Position.Rank + 1
                            : pawn.Position.Rank - 1;

            return pieces.Any(p => p.Position.Rank == passingRank && p.Position.File == pawn.Position.File);
        }

        return false;
    }

    private static bool DirectionIsObstructedForKnight(IEnumerable<Piece> pieces, Piece piece, Square end)
        => pieces.Any(p => p.Color == piece.Color && p.Position == end);

    private static bool DirectionIsObstructed(IEnumerable<Piece> pieces, DirectionType direction, Square start, Square end)
    => direction switch
    {
        DirectionType.Left => pieces.Any(p => (int)p.Position.File < (int)start.File
                              && (int)p.Position.File > (int)end.File
                              && IsSameRank(start, p.Position)),
        DirectionType.Right => pieces.Any(p => (int)p.Position.File > (int)start.File
                               && (int)p.Position.File < (int)end.File
                               && IsSameRank(start, p.Position)),
        DirectionType.Up => pieces.Any(p => p.Position.Rank > start.Rank
                            && p.Position.Rank < end.Rank
                            && IsSameFile(start, p.Position)),
        DirectionType.Down => pieces.Any(p => p.Position.Rank < start.Rank
                              && p.Position.Rank > end.Rank
                              && IsSameFile(start, p.Position)),
        DirectionType.Left | DirectionType.Up => ValidateDiagonalLeftUpObstruction(pieces, start, end),
        DirectionType.Right | DirectionType.Up => ValidateDiagonalRightUpObstruction(pieces, start, end),
        DirectionType.Left | DirectionType.Down => ValidateDiagonalLeftDownObstruction(pieces, start, end),
        DirectionType.Right | DirectionType.Down => ValidateDiagonalRightDownObstruction(pieces, start, end),
        _ => throw new InvalidOperationException("Unknown direction.")
    };

    private static bool ValidateDiagonalRightDownObstruction(IEnumerable<Piece> pieces, Square start, Square end)
    {
        var y = start.File + 1;

        for (var x = start.Rank - 1; x > end.Rank; x--)
        {
            if (pieces.Any(p => p.Position == new Square((File)y, x)))
            {
                return true;
            }

            if (y < end.File)
            {
                y++;
            }
            else break;
        }

        return false;
    }

    private static bool ValidateDiagonalLeftDownObstruction(IEnumerable<Piece> pieces, Square start, Square end)
    {
        var y = start.File - 1;

        for (var x = start.Rank - 1; x > end.Rank; x--)
        {
            var position = new Square((File)y, x);

            if (pieces.Any(p => p.Position == position))
                return true;

            if (y > end.File)
            {
                y--;
            }
            else break;
        }

        return false;
    }

    private static bool ValidateDiagonalRightUpObstruction(IEnumerable<Piece> pieces, Square start, Square end)
    {
        var y = start.File + 1;

        for (var x = start.Rank + 1; x < end.Rank; x++)
        {
            if (pieces.Any(p => p.Position == new Square((File)y, x)))
                return true;

            if (y < end.File)
            {
                y++;
            }
            else break;
        }

        return false;
    }

    private static bool ValidateDiagonalLeftUpObstruction(IEnumerable<Piece> pieces, Square start, Square end)
    {

        var y = start.File - 1;

        for (var x = start.Rank + 1; x < end.Rank; x++)
        {
            var position = new Square(y, x);

            if (pieces.Any(p => p.Position == position))
                return true;

            if (y > end.File)
            {
                y--;
            }
            else break;
        }

        return false;
    }

    private static DirectionType GetVerticalDirection(DirectionType type, Square start, Square end) => start.Rank < end.Rank
                                            ? type |= DirectionType.Up
                                            : type |= DirectionType.Down;

    private static DirectionType GetHorizontalDirection(DirectionType type, Square start, Square end) => start.File < end.File
                                            ? type |= DirectionType.Right
                                            : type |= DirectionType.Left;
}

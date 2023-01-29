using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Model;

public class SpecialMoves
{
    public static bool IsEnPassant(Piece? pawn, IEnumerable<Turn>? turns)
    {
        Guard.Against.Null<Piece?>(pawn, nameof(pawn));
        Guard.Against.Null<IEnumerable<Turn>?>(turns, nameof(turns));

        if (!turns.Any()) return false;

        var lastTurn = turns?.Last();

        if (lastTurn == null && lastTurn?.EndPosition == null && lastTurn?.Player == null)
        {
            return false;
        }

        var isWhiteMove = lastTurn.Player?.Color == Color.White;
        var pawnWasMoved = lastTurn.PieceType == PieceType.Pawn;

        if (!pawnWasMoved) return false;

        var opponentPawnMovedTwoRanks = isWhiteMove
            ? (lastTurn.EndPosition?.Rank - lastTurn.StartPosition?.Rank) > 1
            : (lastTurn.StartPosition?.Rank - lastTurn.EndPosition?.Rank) > 1;

        var enPassantRankPosition = isWhiteMove
            ? lastTurn.EndPosition!.Rank!.Value - 1
            : lastTurn.EndPosition!.Rank!.Value + 1;

        var passedPosition = new Square(lastTurn.EndPosition.File, enPassantRankPosition);

        var isEnPassantMove = pawn.GetAttackRange()
                                  .Where(p => p.File != pawn.Position?.File)
                                  .Any(p => p == passedPosition);

        return opponentPawnMovedTwoRanks && isEnPassantMove;
    }

    public static bool PawnIsPromoted(Piece? piece, Square? endPosition)
    {
        if (piece is Pawn pawn)
        {
            var promotionRank = Color.White == pawn.Color ? 8 : 1;
            return endPosition?.Rank == promotionRank;
        }

        return false;
    }

    public static bool IsCastling(Square? startPosition, Square? endPosition, IEnumerable<Piece>? pieces)
    {
        Guard.Against.Null<Square?>(startPosition, nameof(startPosition));
        Guard.Against.Null<Square?>(endPosition, nameof(endPosition));
        Guard.Against.Null<IEnumerable<Piece>?>(pieces, nameof(pieces));

        var king = pieces.FirstOrDefault(p => p.Position == startPosition);

        if (king == null) return false;

        var rightSideCastling = startPosition?.File == File.E && endPosition?.File == File.G;
        var leftSideCastling = startPosition?.File == File.E && endPosition?.File == File.C;

        return leftSideCastling || rightSideCastling;
    }
}
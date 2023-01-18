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
}
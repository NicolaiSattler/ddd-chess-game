using System.Collections.Generic;
using System.Linq;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Model;

public class SpecialMoves
{
    //TODO: Unit Test
    public static bool IsEnPassant(Piece? pawn, IEnumerable<Turn>? moves)
    {
        _ = pawn ?? throw new ArgumentNullException(nameof(pawn));

        var lastMove = moves?.Last();

        if (lastMove == null && lastMove?.EndPosition == null) return false;

        var isWhiteMove = lastMove.Player?.Color == Color.White;
        var pawnWasMoved = lastMove.PieceType == PieceType.Pawn;

        if (!pawnWasMoved) return false;

        var opponentPawnMovedTwoRanks = isWhiteMove
            ? (lastMove.EndPosition?.Rank - lastMove.StartPosition?.Rank) > 1
            : (lastMove.StartPosition?.Rank - lastMove.EndPosition?.Rank) > 1;

        var enPassantRankPosition = isWhiteMove
            ? lastMove.EndPosition!.Rank!.Value - 1
            : lastMove.EndPosition!.Rank!.Value + 1;

        var passedPosition = new Square(lastMove.EndPosition.File, enPassantRankPosition);

        var isEnPassantMove = pawn.GetAttackRange()
                                  .Where(p => p.File != pawn.Position?.File)
                                  .Any(p => p == passedPosition);

        return opponentPawnMovedTwoRanks && isEnPassantMove;
    }
}
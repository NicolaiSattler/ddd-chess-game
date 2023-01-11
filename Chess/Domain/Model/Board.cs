using System.Collections.Generic;
using System.Linq;
using Chess.Core.Match.Events;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Model;

public class Board
{
    //TODO: Unit Test
    public static bool PieceIsCaptured(TurnTaken @event, IEnumerable<Piece>? pieces)
    {
        var movingPiece = pieces?.FirstOrDefault(p => p.Position == @event.StartPosition)
                        ?? throw new InvalidOperationException("Piece does not exists!");

        var targetPiece = pieces?.FirstOrDefault(p => p.Position == @event.EndPosition);

        return false;
    }

    //TODO: Unit Test
    public static bool PawnIsPromoted(TurnTaken? @event, IEnumerable<Piece>? pieces)
    {
        var movingPiece = pieces?.FirstOrDefault(p => p.Position == @event?.StartPosition)
                ?? throw new InvalidOperationException("Piece cannot be found.");

        if (movingPiece is Pawn pawn)
        {
            var promotionRank = Color.White == pawn.Color ? 8 : 1;
            return @event?.EndPosition?.Rank == promotionRank
                && (pieces?.Any(p => p.Color == pawn.Color && p.Type == PieceType.Queen) ?? false);
        }

        return false;
    }
}
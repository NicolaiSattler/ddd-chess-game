using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Web.Extensions;

public static class PieceExtensions
{
    public static IEnumerable<Square> GetAvailableMoves(this Piece piece, IEnumerable<Piece> pieces)
    {
        var moves = piece.GetAttackRange()
                         .Where(m => !Chess.Domain.Determiners.Board.DirectionIsObstructed(pieces, piece.Position, m))
                         .ToList();

        if (piece is Pawn)
        {
            //TODO: Add En Passant
            var unvalidAttackMoves = moves.Where(m => m.File != piece.Position.File
                                                      && !pieces.Any(p => p.Color != piece.Color && p.Position == m));

            if (unvalidAttackMoves.Any())
            {
                return moves.Except(unvalidAttackMoves);
            }
        }

        return moves;
    }
}
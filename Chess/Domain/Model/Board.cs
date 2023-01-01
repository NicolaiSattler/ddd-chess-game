using System.Collections.Generic;
using System.Linq;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Model;

//Example of Domain Service.
public class Board
{
    private static Func<int?, bool> isValidAxis = (dim) => dim >= 1 && dim <= 8;

    public static IEnumerable<Square> CalculateMovement(Square? position,
                                                        MovementType? movement,
                                                        int range = 2,
                                                        Color color = Color.Undefined)
    {
        if (position == null) throw new ArgumentNullException(nameof(position));
        if (!movement.HasValue) throw new ArgumentNullException(nameof(movement));

        var result = new List<Square>();

        if (movement?.HasFlag(MovementType.Diagonal) ?? false)
            result.AddRange(GetDiagonalMovements(position, range));

        if (movement?.HasFlag(MovementType.FileAndRank) ?? false)
            result.AddRange(GetFileAndRankMovement(position, range));

        if (movement?.HasFlag(MovementType.Leap) ?? false)
            result.AddRange(GetLeapMovements(position));

        if (movement?.HasFlag(MovementType.Pawn) ?? false)
            result.AddRange(GetPawnMovements(position, color));

        return result.Where(p => isValidAxis((int)p.File) && isValidAxis(p.Rank))
                     .OrderBy(p => p.File)
                     .ThenBy(p => p.Rank);
    }

    private static IEnumerable<Square> GetFileAndRankMovement(Square position, int range)
    {
        var result = new List<Square>();

        for (int i = 1; i < range; i++)
        {
            result.Add(new Square(position.File.ChangeFile(-i), position.Rank));
            result.Add(new Square(position.File.ChangeFile(i), position.Rank));
            result.Add(new Square(position.File, position.Rank - i));
            result.Add(new Square(position.File, position.Rank + i));
        }

        return result;
    }


    private static IEnumerable<Square> GetDiagonalMovements(Square position, int range)
    {
        var result = new List<Square>();

        for (int i = 1; i < range; i++)
        {
            result.Add(new Square(position.File.ChangeFile(-i), position.Rank - i));
            result.Add(new Square(position.File.ChangeFile(i), position.Rank - i));
            result.Add(new Square(position.File.ChangeFile(-i), position.Rank + i));
            result.Add(new Square(position.File.ChangeFile(i), position.Rank + i));
        }

        return result;
    }

    private static IEnumerable<Square> GetLeapMovements(Square position) => new List<Square>
    {
        new Square(position.File.ChangeFile(-2), position.Rank - 1),
        new Square(position.File.ChangeFile(-1), position.Rank - 2),
        new Square(position.File.ChangeFile(2), position.Rank + 1),
        new Square(position.File.ChangeFile(1), position.Rank + 2),
        new Square(position.File.ChangeFile(-2), position.Rank + 1),
        new Square(position.File.ChangeFile(-1), position.Rank + 2),
        new Square(position.File.ChangeFile(2), position.Rank - 1),
        new Square(position.File.ChangeFile(1), position.Rank - 2)
    };


    private static IEnumerable<Square> GetPawnMovements(Square? position, Color color)
    {
        var rank = color == Color.Black ? position?.Rank - 1 : position?.Rank + 1;

        if (position?.Rank == 2 || position?.Rank == 7)
        {
            var increaseRank = position.Rank == 2;
            var result = new List<Square>()
            {
                new Square(position.File, position.Rank + (increaseRank ? 1 : -1)),
                new Square(position.File.ChangeFile(1), position.Rank + (increaseRank ? 1 : -1)),
                new Square(position.File.ChangeFile(-1), position.Rank + (increaseRank ? 1 : -1)),
                new Square(position.File, position.Rank + (increaseRank ? 2 : -2))
            };
            return result;
        }

        return position != null
            ? new List<Square>()
              {
                  new Square(position.File, rank),
                  new Square(position.File.ChangeFile(-1), rank),
                  new Square(position.File.ChangeFile(1), rank),
              }
            : Enumerable.Empty<Square>();
    }


    private static IEnumerable<Square> GetLegalMovements(IEnumerable<Piece> pieces, Piece movingPiece, int range = 1)
    {
        var result = new List<Square>();
        var piecesOfSameColor = pieces.Where(p => p.Color == movingPiece.Color);

        if (movingPiece.Type == PieceType.Knight)
        {
            var possibleMoves = movingPiece.GetAttackRange()
                                           .Where(m => piecesOfSameColor.Any(p => p.Position != m));

            return possibleMoves;
        }
        else
        {
            var moves = movingPiece.GetAttackRange();

            //TODO: remove moves obstructed by own pieces.
            //check each dimension.
            //filter all squares greater than dimension of obstructive piece

            return moves;
        }
    }


}

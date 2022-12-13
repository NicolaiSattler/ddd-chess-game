using System.Collections.Generic;
using System.Linq;
using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match.Entities;

public class AttackRangeHelper
{
    private static Func<int, bool> isValidAxis = (dim) => dim < 1 || dim > 8;

    public static IEnumerable<Square> CalculateMovement(Square position,
                                                        MovementType movement,
                                                        int range = 1,
                                                        Color color = Color.Undefined)
    {
        var result = new List<Square>();

        if (movement.HasFlag(MovementType.Diagonal))
            result.AddRange(GetDiagonalMovements(position, range));

        if (movement.HasFlag(MovementType.Rectangular))
            result.AddRange(GetRectangularMovements(position, range));

        if (movement.HasFlag(MovementType.Leap))
            result.AddRange(GetLeapMovements(position));

        if (movement.HasFlag(MovementType.Pawn))
            result.AddRange(GetPawnMovements(position, color));

        return result.Where(p => isValidAxis(p.Colomn) && isValidAxis(p.Row));
    }

    private static IEnumerable<Square> GetRectangularMovements(Square position, int range)
    {
        var result = new List<Square>();

        for (int i = 0; i < range; i++)
        {
            result.Add(new Square(position.Colomn - range, position.Row));
            result.Add(new Square(position.Colomn + range, position.Row));
            result.Add(new Square(position.Colomn, position.Row - range));
            result.Add(new Square(position.Colomn, position.Row + range));
        }

        return result;
    }

    private static IEnumerable<Square> GetDiagonalMovements(Square position, int range)
    {
        var result = new List<Square>();

        for (int i = 0; i < range; i++)
        {
            result.Add(new Square(position.Colomn - range, position.Row - range));
            result.Add(new Square(position.Colomn + range, position.Row - range));
            result.Add(new Square(position.Colomn - range, position.Row + range));
            result.Add(new Square(position.Colomn + range, position.Row + range));
        }

        return result;
    }

    private static IEnumerable<Square> GetLeapMovements(Square position) => new List<Square>
    {
        new Square(position.Colomn - 2, position.Row - 1),
        new Square(position.Colomn - 1, position.Row - 2),
        new Square(position.Colomn + 2, position.Row + 1),
        new Square(position.Colomn + 1, position.Row + 2),
        new Square(position.Colomn - 2, position.Row + 1),
        new Square(position.Colomn - 1, position.Row + 2),
        new Square(position.Colomn + 2, position.Row - 1),
        new Square(position.Colomn + 1, position.Row - 2)
    };


    private static IEnumerable<Square> GetPawnMovements(Square position, Color color)
    {
        var row = color == Color.Black ? position.Row - 1 : position.Row + 1;

        return new List<Square>()
        {
            new Square(position.Colomn, row),
            new Square(position.Colomn - 1, row),
            new Square(position.Colomn + 1, row),
        };
    }

    private static IEnumerable<Square> GetLegalMovements(IEnumerable<Piece> pieces, Piece movingPiece, int range = 1)
    {
        var result = new List<Square>();

        if (movingPiece.Type == PieceType.Knight)
        {
            var piecesOfSameColor = pieces.Where(p => p.Color == movingPiece.Color);
            var possibleMoves = movingPiece.GetAttackRange()
                                           .Where(m => piecesOfSameColor.Any(p => p.Position != m));

            return possibleMoves;
        }
        else
        {
            //TODO: recursive function.
            var bla = CalculateMovement(movingPiece.Position, movingPiece.Movement, 1);

            throw new NotImplementedException("....")

        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Chess.Core.Match.Entities;
using Chess.Core.Match.ValueObjects;

namespace Chess.Core.Match;

//Example of Domain Service.
public class Board
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

        if (movement.HasFlag(MovementType.FileAndRank))
            result.AddRange(GetFileAndRankMovement(position, range));

        if (movement.HasFlag(MovementType.Leap))
            result.AddRange(GetLeapMovements(position));

        if (movement.HasFlag(MovementType.Pawn))
            result.AddRange(GetPawnMovements(position, color));

        return result.Where(p => isValidAxis(p.File) && isValidAxis(p.Rank));
    }

    private static IEnumerable<Square> GetFileAndRankMovement(Square position, int range)
    {
        var result = new List<Square>();

        for (int i = 0; i < range; i++)
        {
            result.Add(new Square(position.File - range, position.Rank));
            result.Add(new Square(position.File + range, position.Rank));
            result.Add(new Square(position.File, position.Rank - range));
            result.Add(new Square(position.File, position.Rank + range));
        }

        return result;
    }


    private static IEnumerable<Square> GetDiagonalMovements(Square position, int range)
    {
        var result = new List<Square>();

        for (int i = 0; i < range; i++)
        {
            result.Add(new Square(position.File - range, position.Rank - range));
            result.Add(new Square(position.File + range, position.Rank - range));
            result.Add(new Square(position.File - range, position.Rank + range));
            result.Add(new Square(position.File + range, position.Rank + range));
        }

        return result;
    }

    private static IEnumerable<Square> GetLeapMovements(Square position) => new List<Square>
    {
        new Square(position.File - 2, position.Rank - 1),
        new Square(position.File - 1, position.Rank - 2),
        new Square(position.File + 2, position.Rank + 1),
        new Square(position.File + 1, position.Rank + 2),
        new Square(position.File - 2, position.Rank + 1),
        new Square(position.File - 1, position.Rank + 2),
        new Square(position.File + 2, position.Rank - 1),
        new Square(position.File + 1, position.Rank - 2)
    };


    private static IEnumerable<Square> GetPawnMovements(Square position, Color color)
    {
        var rank = color == Color.Black ? position.Rank - 1 : position.Rank + 1;

        return new List<Square>()
        {
            new Square(position.File, rank),
            new Square(position.File - 1, rank),
            new Square(position.File + 1, rank),
        };
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

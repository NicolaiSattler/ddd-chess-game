using System.Collections.Generic;
using Chess.Core.Match.ValueObjects;

namespace Chess.Core.Match.Entities.Pieces;

public class Bishop : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Bishop(Guid id) : base(id)
    {
        Type = PieceType.Bishop;
        Movement = MovementType.Diagonal;
    }


    public override IEnumerable<Square> GetAttackRange() => Board.CalculateMovement(Position, Movement, 7);

}

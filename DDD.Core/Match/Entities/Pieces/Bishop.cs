using System.Collections.Generic;
using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match.Entities.Pieces;

public class Bishop : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Bishop(Guid id) : base(id)
    {
        Type = PieceType.Bishop;
        Movement = MovementType.Diagonal;
    }


    public override IEnumerable<Square> AttackRange() => AttackRangeHelper.CalculateMovement(Position, Movement, 7);

}
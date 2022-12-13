using System.Collections.Generic;
using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match.Entities.Pieces;

public class King : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public King(Guid id) : base(id)
    {
        Type = PieceType.King;
        Movement = MovementType.Diagonal | MovementType.Rectangular;
    }

    public override IEnumerable<Square> GetAttackRange() => AttackRangeHelper.CalculateMovement(Position, Movement);
}
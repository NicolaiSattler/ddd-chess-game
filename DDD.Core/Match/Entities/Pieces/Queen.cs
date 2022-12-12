using System.Collections.Generic;
using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match.Entities.Pieces;

public class Queen : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Queen(Guid id) : base(id)
    {
        Type = PieceType.Queen;
        Movement = MovementType.Diagonal | MovementType.Rectangular;
    }

    public override IEnumerable<Square> AttackRange() => AttackRangeHelper.CalculateMovement(Position, Movement, 7);
}
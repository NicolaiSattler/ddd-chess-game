using System.Collections.Generic;
using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match.Entities.Pieces;

public class Pawn : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Pawn(Guid id) : base(id)
    {
        Type = PieceType.Pawn;
        Movement = MovementType.Pawn;
    }

    public override IEnumerable<Square> GetAttackRange() => AttackRangeHelper.CalculateMovement(Position, Movement, color: Color);
}
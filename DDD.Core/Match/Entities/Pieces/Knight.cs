using System.Collections.Generic;
using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match.Entities.Pieces;

public class Knight : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Knight(Guid id) : base(id)
    {
        Type = PieceType.Knight;
        Movement = MovementType.Leap;
    }

    public override IEnumerable<Square> GetAttackRange() => Board.CalculateMovement(Position, Movement);

}

using System.Collections.Generic;
using Chess.Domain.Utilities;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public class Knight : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Knight() : this(Guid.NewGuid()) { }
    public Knight(Guid id) : base(id)
    {
        Type = PieceType.Knight;
        Movement = MovementType.Leap;
    }

    public override IEnumerable<Square> GetAttackRange() => Navigator.CalculateMovement(Position, Movement);

}

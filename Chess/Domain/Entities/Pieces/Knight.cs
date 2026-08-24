using System.Collections.Generic;
using Chess.Domain.Utilities;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public class Knight(Guid id) : Piece(id)
{
    public override PieceType Type { get; init; } = PieceType.Knight;

    private MovementType movement = MovementType.Leap;

    public override MovementType GetMovement()
    {
        return movement;
    }

    public override void SetMovement(MovementType value)
    {
        this.movement = value;
    }

    public Knight() : this(Guid.NewGuid()) { }

    public override IEnumerable<Square> GetAttackRange() => Navigator.CalculateMovement(Position, GetMovement());

}

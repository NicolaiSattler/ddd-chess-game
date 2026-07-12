using System.Collections.Generic;
using Chess.Domain.Utilities;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public class Bishop : Piece
{
    public override PieceType Type { get; init; }

    private readonly MovementType movement;

    public override MovementType GetMovement()
    {
        return movement;
    }

    public override void SetMovement(MovementType value)
    {
        this.movement = value;
    }

    public Bishop() : this(Guid.NewGuid()) { }
    public Bishop(Guid id) : base(id)
    {
        Type = PieceType.Bishop;
        SetMovement(MovementType.Diagonal);
    }

    public override IEnumerable<Square> GetAttackRange() => Navigator.CalculateMovement(Position, GetMovement(), 8);
}

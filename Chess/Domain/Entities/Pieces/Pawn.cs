using System.Collections.Generic;
using Chess.Domain.Utilities;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public class Pawn : Piece
{
    public override PieceType Type { get; init; }

    private MovementType movement;

    public override MovementType GetMovement()
    {
        return movement;
    }

    public override void SetMovement(MovementType value)
    {
        this.movement = value;
    }

    public Pawn() : base(Guid.NewGuid())
    {
        Type = PieceType.Pawn;
        SetMovement(MovementType.Pawn);
    }

    public override IEnumerable<Square> GetAttackRange() => Navigator.CalculateMovement(Position, GetMovement(), color: Color);
}

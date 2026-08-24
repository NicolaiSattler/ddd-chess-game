using System.Collections.Generic;
using Chess.Domain.Utilities;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public class Queen : Piece
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

    public Queen() : this(Guid.NewGuid()) { }

    public Queen(Guid id) : base(id)
    {
        Type = PieceType.Queen;
        SetMovement(MovementType.Diagonal | MovementType.FileAndRank);
    }

    public override IEnumerable<Square> GetAttackRange() => Navigator.CalculateMovement(Position, GetMovement(), 8);
}

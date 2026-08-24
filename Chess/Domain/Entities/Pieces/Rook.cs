using System.Collections.Generic;
using Chess.Domain.Utilities;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public class Rook : Piece
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

    public Rook() : this(Guid.NewGuid()) { }
    public Rook(Guid id) : base(id)
    {
        Type = PieceType.Rook;
        SetMovement(MovementType.FileAndRank);
    }

    public override IEnumerable<Square> GetAttackRange() => Navigator.CalculateMovement(Position, GetMovement(), 8);
}

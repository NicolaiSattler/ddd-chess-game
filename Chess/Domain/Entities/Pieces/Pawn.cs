using System.Collections.Generic;
using Chess.Domain.Model;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public class Pawn : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Pawn() : base(Guid.NewGuid())
    {
        Type = PieceType.Pawn;
        Movement = MovementType.Pawn;
    }

    public override IEnumerable<Square> GetAttackRange() => Navigator.CalculateMovement(Position, Movement, color: Color);
}

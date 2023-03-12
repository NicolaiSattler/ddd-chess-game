using System.Collections.Generic;
using Chess.Domain.Utilities;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public class Queen : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Queen() : base(Guid.NewGuid())
    {
        Type = PieceType.Queen;
        Movement = MovementType.Diagonal | MovementType.FileAndRank;
    }

    public override IEnumerable<Square> GetAttackRange() => Navigator.CalculateMovement(Position, Movement, 8);
}

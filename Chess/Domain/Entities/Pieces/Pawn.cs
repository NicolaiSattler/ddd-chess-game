using System.Collections.Generic;
using Chess.Core.Match.ValueObjects;

namespace Chess.Core.Match.Entities.Pieces;

public class Pawn : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Pawn(Guid id) : base(id)
    {
        Type = PieceType.Pawn;
        Movement = MovementType.Pawn;
    }

    public override IEnumerable<Square> GetAttackRange() => Board.CalculateMovement(Position, Movement, color: Color);
}

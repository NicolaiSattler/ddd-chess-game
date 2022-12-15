using System.Collections.Generic;
using Chess.Core.Match.ValueObjects;

namespace Chess.Core.Match.Entities.Pieces;

public class King : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public King(Guid id) : base(id)
    {
        Type = PieceType.King;
        Movement = MovementType.Diagonal | MovementType.FileAndRank;
    }

    public override IEnumerable<Square> GetAttackRange() => Board.CalculateMovement(Position, Movement);
}

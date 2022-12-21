using System.Collections.Generic;
using Chess.Core.Match.ValueObjects;
using Chess.Domain.Model;

namespace Chess.Domain.Entities.Pieces;

public class King : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public King() : base(Guid.NewGuid())
    {
        Type = PieceType.King;
        Movement = MovementType.Diagonal | MovementType.FileAndRank;
    }

    public override IEnumerable<Square> GetAttackRange() => Board.CalculateMovement(Position, Movement);
}

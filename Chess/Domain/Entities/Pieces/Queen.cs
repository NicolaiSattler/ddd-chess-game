using System.Collections.Generic;
using Chess.Core.Match.ValueObjects;
using Chess.Domain.Model;

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

    public override IEnumerable<Square> GetAttackRange() => Board.CalculateMovement(Position, Movement, 8);
}

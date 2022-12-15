using System.Collections.Generic;
using Chess.Core.Match.ValueObjects;

namespace Chess.Core.Match.Entities.Pieces;

public class Queen : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Queen(Guid id) : base(id)
    {
        Type = PieceType.Queen;
        Movement = MovementType.Diagonal | MovementType.FileAndRank;
    }

    public override IEnumerable<Square> GetAttackRange() => Board.CalculateMovement(Position, Movement, 7);
}

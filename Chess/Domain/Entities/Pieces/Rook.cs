using System.Collections.Generic;
using Chess.Core.Match.ValueObjects;

namespace Chess.Core.Match.Entities.Pieces;

public class Rook : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Rook(Guid id) : base(id)
    {
        Type = PieceType.Rook;
        Movement = MovementType.FileAndRank;
    }

    public override IEnumerable<Square> GetAttackRange() => Board.CalculateMovement(Position, Movement, 7);
}

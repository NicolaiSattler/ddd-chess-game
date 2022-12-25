using System.Collections.Generic;
using Chess.Domain.Model;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public class Rook : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Rook() : base(Guid.NewGuid())
    {
        Type = PieceType.Rook;
        Movement = MovementType.FileAndRank;
    }

    public override IEnumerable<Square> GetAttackRange() => Board.CalculateMovement(Position, Movement, 8);
}

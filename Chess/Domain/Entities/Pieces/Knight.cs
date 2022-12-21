using System.Collections.Generic;
using Chess.Core.Match.ValueObjects;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Model;

namespace Chess.Domain.Entities.Pieces;

public class Knight : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Knight() : base(Guid.NewGuid())
    {
        Type = PieceType.Knight;
        Movement = MovementType.Leap;
    }

    public override IEnumerable<Square> GetAttackRange() => Board.CalculateMovement(Position, Movement);

}

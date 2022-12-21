using System.Collections.Generic;
using Chess.Core.Match.ValueObjects;
using Chess.Domain.Model;

namespace Chess.Domain.Entities.Pieces;

public class Bishop : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Bishop() : base(Guid.NewGuid())
    {
        Type = PieceType.Bishop;
        Movement = MovementType.Diagonal;
    }


    public override IEnumerable<Square> GetAttackRange() => Board.CalculateMovement(Position, Movement, 8);

}

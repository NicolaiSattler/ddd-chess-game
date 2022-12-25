using System.Collections.Generic;
using Chess.Domain.Model;
using Chess.Domain.ValueObjects;

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

using System.Collections.Generic;
using Chess.Domain.Utilities;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public class Bishop : Piece
{
    public override PieceType Type { get; init; }
    public override MovementType Movement { get; init; }

    public Bishop() : this(Guid.NewGuid()) { }
    public Bishop(Guid id) : base(id)
    {
        Type = PieceType.Bishop;
        Movement = MovementType.Diagonal;
    }


    public override IEnumerable<Square> GetAttackRange() => Navigator.CalculateMovement(Position, Movement, 8);

}

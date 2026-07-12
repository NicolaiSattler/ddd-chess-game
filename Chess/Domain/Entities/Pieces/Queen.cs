using System.Collections.Generic;
using Chess.Domain.Utilities;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public class Queen(Guid id) : Piece(id)
{
    public override PieceType Type { get; init; } = PieceType.Queen;
    public override MovementType Movement { get; init; } = MovementType.Diagonal | MovementType.FileAndRank;

    public Queen() : this(Guid.NewGuid()) { }

    public override IEnumerable<Square> GetAttackRange() => Navigator.CalculateMovement(Position, Movement, 8);
}

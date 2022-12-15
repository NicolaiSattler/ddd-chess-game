using System.Collections.Generic;
using Chess.Core.Match.ValueObjects;

namespace Chess.Core.Match.Entities;

public abstract class Piece : Entity<Guid>
{
    public Color Color { get; init; }
    public Square Position { get; set; }
    public abstract PieceType Type { get; init; }
    public abstract MovementType Movement { get; init; }

    public Piece(Guid id) : base(id) { }

    public abstract IEnumerable<Square> GetAttackRange();
}

public enum MovementType
{
    Undefined = 0,
    Diagonal = 1,
    FileAndRank = 2,
    Leap = 3,
    Pawn = 4,
}

public enum PieceType
{
    Undefined = 0,
    King = 1,
    Queen = 2,
    Rook = 3,
    Knight = 4,
    Bishop = 5,
    Pawn = 6
}

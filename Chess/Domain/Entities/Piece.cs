using System.Collections.Generic;
using Chess.Core;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities.Pieces;

public abstract class Piece : Entity
{
    public Color Color { get; init; }
    public Square Position { get; private set; } = new(File.Undefined, 0);
    public abstract PieceType Type { get; init; }
    public abstract MovementType GetMovement();
    public abstract void SetMovement(MovementType value);

    public Piece(Guid id) : base(id)
    {
        Position = new(File.Undefined, 0);
    }

    public Piece(Guid id, Square position) : base(id)
    {
        Position = position;
    }

    public void MoveTo(Square position)
    {
        Position = position;
    }

    public abstract IEnumerable<Square> GetAttackRange();

    public override string ToString() => $"{Type}{Position}";
}

public enum MovementType
{
    Undefined = 0,
    Diagonal = 1,
    FileAndRank = 2,
    Leap = 4,
    Pawn = 8,
}

public enum DirectionType
{
    Undefined = 0,
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8,
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

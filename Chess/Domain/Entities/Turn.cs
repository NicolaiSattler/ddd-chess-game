using Chess.Core;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities;

public class Turn : Entity
{
    public PieceType PieceType { get; private set; }
    public Square? StartPosition { get; private set; }
    public Square? EndPosition { get; private set; }
    public DateTime StartTime { get; init; }
    public Player Player { get; init; } = new();
    public string Hash { get; private set; } = string.Empty;
    public string Notation { get; private set; } = string.Empty;

    public Turn() : base(Guid.NewGuid()) { }

    public Turn(PieceType pieceType, Square? startPosition, Square? endPosition, string hash, string notation) 
        : base(Guid.NewGuid())
    {
        PieceType = pieceType;
        StartPosition = startPosition;
        EndPosition = endPosition;
        Hash = hash;
        Notation = notation;
        StartTime = DateTime.UtcNow;
    }

    public void UpdateMoveData(PieceType pieceType, Square? startPosition, Square? endPosition, string hash, string notation)
    {
        PieceType = pieceType;
        StartPosition = startPosition;
        EndPosition = endPosition;
        Hash = hash;
        Notation = notation;
    }

    public void UpdateNotation(string notation)
    {
        Notation = notation;
    }
}

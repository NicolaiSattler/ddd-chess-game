using Chess.Core;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities;

public class Turn : Entity
{
    public PieceType PieceType { get; set; }
    public Square? StartPosition { get; set; }
    public Square? EndPosition { get; set; }
    public DateTime StartTime { get; init; }
    public Player? Player { get; init; }
    public string Hash { get; set; } = string.Empty;
    public string Notation { get; set; } = string.Empty;

    public Turn() : base(Guid.NewGuid()) { }
}

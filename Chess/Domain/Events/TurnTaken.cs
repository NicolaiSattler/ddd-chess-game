using Chess.Core;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Events;

public class TurnTaken : DomainEvent
{
    public Guid MemberId { get; init; }
    public Square StartPosition { get; init; } = Square.Empty();
    public Square EndPosition { get; init; } = Square.Empty();
    public DateTime EndTime { get; init; }
}

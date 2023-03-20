using Ardalis.GuardClauses;
using Chess.Core;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Events;

public class TurnTaken : DomainEvent
{
    public Guid MemberId { get; }
    public Square StartPosition { get; }
    public Square EndPosition { get; }

    public TurnTaken(Guid memberId, Square startPosition, Square endPosition)
    {
        MemberId = memberId;
        StartPosition = Guard.Against.Null<Square>(startPosition, nameof(startPosition));
        EndPosition = Guard.Against.Null<Square>(endPosition, nameof(endPosition));
    }
}

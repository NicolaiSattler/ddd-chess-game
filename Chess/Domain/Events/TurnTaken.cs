using Ardalis.GuardClauses;
using Chess.Core;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Events;

public class TurnTaken : DomainEvent
{
    public Guid MemberId { get; init; }
    public Square StartPosition { get; init; }
    public Square EndPosition { get; init; }
    public DateTime EndTime { get; init; }

    public TurnTaken(Guid memberId, Square startPosition, Square endPosition, DateTime endTime)
    {
        MemberId = memberId;
        StartPosition = Guard.Against.Null(startPosition, nameof(startPosition));
        EndPosition = Guard.Against.Null(endPosition, nameof(endPosition));
        EndTime = endTime;
    }
}

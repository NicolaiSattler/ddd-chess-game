using Chess.Core.Match.ValueObjects;

namespace Chess.Core.Match.Events;

public class TurnTaken : DomainEvent
{
    public readonly Guid MemberId;
    public readonly Square StartPosition;
    public readonly Square EndPosition;

    public TurnTaken(Guid memberId, Square startPosition, Square endPosition)
    {
        MemberId = memberId;
        StartPosition = startPosition;
        EndPosition = endPosition;
    }
}
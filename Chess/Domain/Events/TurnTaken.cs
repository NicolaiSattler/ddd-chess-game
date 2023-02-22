using Chess.Core;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Events;

public class TurnTaken : DomainEvent
{
    public Guid MemberId { get; }
    public Square? StartPosition { get; }
    public Square? EndPosition { get; }

    public TurnTaken(Guid memberId, Square? startPosition, Square? endPosition)
    {
        MemberId = memberId;
        StartPosition = startPosition;
        EndPosition = endPosition;
    }
}

using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match.Events;

public class TurnTaken : DomainEvent
{
    public readonly Guid MemberId;
    public readonly Guid PieceId;
    public readonly Square Position;

    public TurnTaken(Guid memberId, Guid pieceId, Square position)
    {
        MemberId = memberId;
        PieceId = pieceId;
        Position = position;
    }
}
namespace DDD.Core.Match.Events;

public class MatchStarted: DomainEvent
{
    public readonly Guid WhiteMemberId;
    public readonly Guid BlackMemberId;
    public readonly DateTime StartTime;

    public MatchStarted(Guid whiteMemberId,
                        Guid blackMemberId,
                        DateTime startTime)
    {
        WhiteMemberId = whiteMemberId;
        BlackMemberId = blackMemberId;
        StartTime = startTime;
    }
}
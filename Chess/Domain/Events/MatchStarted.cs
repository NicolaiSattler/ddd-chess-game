namespace Chess.Core.Match.Events;

[Serializable]
public class MatchStarted: DomainEvent
{
    public Guid WhiteMemberId { get; set; }
    public Guid BlackMemberId { get; set; }
    public DateTime StartTime { get; set; }

    public MatchStarted(Guid whiteMemberId,
                        Guid blackMemberId,
                        DateTime startTime)
    {
        WhiteMemberId = whiteMemberId;
        BlackMemberId = blackMemberId;
        StartTime = startTime;
    }
}
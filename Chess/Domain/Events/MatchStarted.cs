namespace Chess.Core.Match.Events;

[Serializable]
public class MatchStarted : DomainEvent
{
    public Guid WhiteMemberId { get; init; }
    public Guid BlackMemberId { get; init; }
    public float EloOfWhite { get; init; }
    public float EloOfBlack { get; init; }
    public DateTime StartTime { get; init; }
}

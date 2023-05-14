using Chess.Core;
using Chess.Domain.Configuration;

namespace Chess.Domain.Events;

[Serializable]
public class MatchStarted : DomainEvent
{
    public Guid AggregateId { get; init; }
    public Guid WhiteMemberId { get; init; }
    public Guid BlackMemberId { get; init; }
    public float EloOfWhite { get; init; }
    public float EloOfBlack { get; init; }
    public DateTime StartTime { get; init; }
    public MatchOptions Options { get; init; } = new();
}

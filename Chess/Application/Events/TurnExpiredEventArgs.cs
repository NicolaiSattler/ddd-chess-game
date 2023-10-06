namespace Chess.Application.Events;

public record TurnExpiredEventArgs
{
    public Guid AggregateId { get; init; }
    public Guid MemberId { get; init; }
    public TimeSpan ExceededTime { get; init; }
}
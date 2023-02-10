namespace Chess.Core.Match.Events;

public class MatchEnded : DomainEvent
{
    public Guid MemberOneId { get; init; }
    public Guid MemberTwoId { get; init; }
    public int MemberOneElo { get; init; }
    public int MemberTwoElo { get; init; }
}

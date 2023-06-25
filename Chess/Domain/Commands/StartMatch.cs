using Chess.Domain.Configuration;

namespace Chess.Domain.Commands;

public record StartMatch
{
    public Guid AggregateId { get; init; }
    public Guid MemberOneId { get; init; }
    public Guid MemberTwoId { get; init; }
    public MatchOptions Options { get; init; } = new();
}

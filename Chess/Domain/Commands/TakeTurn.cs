using Chess.Core.Match.ValueObjects;

namespace Chess.Core.Match.Commands;

public record TakeTurn
{
    public Guid MemberId { get; init; }
    public Square? StartPosition { get; init; }
    public Square? EndPosition { get; init; }
}
using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match.Commands;

public record TakeTurn
{
    public Guid MemberId { get; init; }
    public Square Piece { get; init; }
    public Square NewPosition { get; init; }
}
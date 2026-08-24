using Chess.Domain.ValueObjects;

namespace Chess.Domain.Commands;

public record TakeTurn
{
    public Guid MemberId { get; init; }
    public Square StartPosition { get; init; } = Square.Empty();
    public Square EndPosition { get; init; } = Square.Empty();
    /// <summary>
    /// Expected version of the aggregate for optimistic concurrency control.
    /// If specified and doesn't match current version, returns concurrency error.
    /// </summary>
    public int? ExpectedVersion { get; init; }
}

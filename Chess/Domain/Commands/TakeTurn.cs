using Chess.Domain.ValueObjects;

namespace Chess.Domain.Commands;

public record TakeTurn
{
    public Guid MemberId { get; init; }
    public Square? StartPosition { get; init; }
    public Square? EndPosition { get; init; }
    public bool? IsCastling { get; init; }
}
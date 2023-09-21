namespace Chess.Domain.Commands;

public record Surrender
{
    public Guid MemberId { get; init; }
}

namespace Chess.Domain.Commands;

public record Resign
{
    public Guid MemberId { get; init; }
}

namespace Chess.Domain.Commands;

public record Forfeit
{
    public Guid MemberId { get; init; }
}

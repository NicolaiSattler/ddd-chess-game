namespace Chess.Domain.Commands;

public record ForfeitCommand
{
    public Guid MemberId { get; init; }
}

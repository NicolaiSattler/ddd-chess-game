namespace Chess.Domain.Commands;

public record ProposeDraw()
{
    public Guid MemberId { get; init; }
}
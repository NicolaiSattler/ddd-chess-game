namespace Chess.Domain.Commands;

public record Draw
{
    public bool Accepted { get; init; }
}


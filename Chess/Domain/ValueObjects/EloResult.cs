namespace Chess.Domain.ValueObjects;

public record EloResult
{
    public float WhiteElo { get; init; }
    public float BlackElo { get; init; }
}


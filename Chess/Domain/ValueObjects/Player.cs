namespace Chess.Domain.ValueObjects;

public record Player
{
    public Guid MemberId { get; init; }
    public float Elo { get; init; }
    public Color Color { get; init; }
}

public enum Color
{
    Undefined = 0,
    Black = 1,
    White = 2
}

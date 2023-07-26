namespace Chess.Domain.ValueObjects;

public record Square(File File, int Rank)
{
    public override string ToString() => $"{File}{Rank}";

    public static Square Empty() => new(File.Undefined, 0);
}

public enum File
{
    Undefined = 0,
    A = 1,
    B = 2,
    C = 3,
    D = 4,
    E = 5,
    F = 6,
    G = 7,
    H = 8
}

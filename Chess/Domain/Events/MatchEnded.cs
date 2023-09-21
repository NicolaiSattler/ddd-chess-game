using Ardalis.GuardClauses;
using Chess.Core;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Events;

public enum MatchResult
{
    Undefined = 0,
    WhiteWins = 1,
    BlackWins = 2,
    Draw = 3,
    Stalemate = 4,
    WhiteSurrenders = 5,
    BlackSurrenders = 6
}

public class MatchEnded : DomainEvent
{
    public Player White { get; }
    public Player Black { get; }
    public MatchResult Result { get; }

    public MatchEnded(Player? white, Player? black, MatchResult result)
    {
        White = Guard.Against.Null(white, nameof(white))!;
        Black = Guard.Against.Null(black, nameof(black))!;
        Result = result;
    }
}

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
    WhiteForfeit = 5,
    BlackForfeit = 6
}

public class MatchEnded : DomainEvent
{
    public Player White { get; }
    public Player Black { get; }
    public MatchResult Result { get; }

    public MatchEnded(Player? white, Player? black, MatchResult result)
    {
        White = Guard.Against.Null<Player?>(white, nameof(white))!;
        Black = Guard.Against.Null<Player?>(black, nameof(black))!;
        Result = result;
    }
}

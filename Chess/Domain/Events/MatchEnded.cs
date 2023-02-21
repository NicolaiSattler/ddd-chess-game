using Chess.Core;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Events;

public enum MatchResult
{
    White = 0,
    Black = 1,
    Draw = 2
}

public class MatchEnded : DomainEvent
{
    public Player? White { get; }
    public Player? Black { get; }
    public MatchResult Result { get; }

    public MatchEnded(Player? white, Player? black, MatchResult result)
    {
        White = white;
        Black = black;
        Result = result;
    }
}

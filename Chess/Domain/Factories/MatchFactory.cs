using Chess.Domain.Aggregates;

namespace Chess.Domain.Factories;

public class MatchFactory
{
    public static Match Create() => new Match(Guid.NewGuid());
}

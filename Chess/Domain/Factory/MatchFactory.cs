namespace Chess.Domain;

public class MatchFactory
{
    public static Match Create() => new Match(Guid.NewGuid());
}

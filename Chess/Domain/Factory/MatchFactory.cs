namespace Chess.Core.Match;

public class MatchFactory
{
    public static Match Create() => new Match(Guid.NewGuid());


}

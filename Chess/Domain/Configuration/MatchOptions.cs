namespace Chess.Domain.Configuration;

public sealed class MatchOptions
{
    public TimeSpan MaxTurnTime { get; set; }
    public bool DrawByRepition { get; set; }
}

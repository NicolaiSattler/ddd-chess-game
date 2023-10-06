namespace Chess.Domain.Configuration;

public sealed class MatchOptions
{
    public const string SectionName = "MatchOptions";

    public TimeSpan MaxTurnTime { get; set; } = TimeSpan.MinValue;
    public bool UseTurnTimer { get; set; }
    public bool DrawByRepition { get; set; }
}

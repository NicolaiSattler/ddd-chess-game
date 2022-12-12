using System;

namespace DDD.Core.Match.Commands;

public record EndMatch
{
    public Guid WinnerId { get; init; }
    public Guid LoserId { get; init; }

    public double WinnerEloScore { get; init; }
    public double LoserEloScore { get; init; }
}

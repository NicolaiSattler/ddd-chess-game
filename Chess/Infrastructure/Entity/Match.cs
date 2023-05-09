using System.Collections.Generic;

namespace Chess.Infrastructure.Entity;

public record Match
{
    public Guid AggregateId { get; init; }
    public Guid BlackPlayerId { get; init; }
    public Guid WhitePlayerId { get; init; }
    public DateTime StartTime { get; init; }
    public string? Options { get; init; }

    public List<MatchEvent>? Events { get; set; }
}

using System.Collections.Generic;
using Chess.Domain.Configuration;

namespace Chess.Infrastructure.Entity;

public record Match
{
    public Guid AggregateId { get; init; }
    public Guid BlackPlayerId { get; init; }
    public Guid WhitePlayerId { get; init; }
    public DateTime StartTime { get; init; }
    public MatchOptions? Options { get; init; }

    public List<MatchEvent>? Events { get; set; }
}

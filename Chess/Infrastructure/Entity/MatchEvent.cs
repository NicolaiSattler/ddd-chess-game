using Chess.Core;
using Chess.Infrastructure.Extensions;

namespace Chess.Infrastructure.Entity;

public record MatchEvent
{
    private DomainEvent? _event;

    public Guid Id { get; init; }
    public Guid AggregateId { get; init; }
    public int Version { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Data { get; init; } = string.Empty;
    public DomainEvent? Event => _event ??= this?.ToDomainEvent();
    public DateTime CreatedAtUtc { get; init; }
    public Match? Match { get; init; }
}

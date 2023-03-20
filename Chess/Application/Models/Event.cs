namespace Chess.Application;

public record Event
{
    public Guid Id { get; init; }
    public Guid AggregateId { get; init; }
    public int Version { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Data { get; init; } = string.Empty;
}

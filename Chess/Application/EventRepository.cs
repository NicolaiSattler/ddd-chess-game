using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Chess.Core;
using Chess.Core.Match;
using Chess.Core.Match.Events;

namespace Chess.Application;

public class MatchRepository
{

    private readonly Dictionary<Guid, List<Event>> _events = new();


    public Match? Get(Guid aggregateId)
    {
        if (_events.TryGetValue(aggregateId, out List<Event>? aggregateEvents))
        {
            var events = aggregateEvents.OrderBy(e => e.Version).Select(DeserializeEvent);
            return new Match(aggregateId, events);
        }

        return default;
    }


    private static Event CreateEvent(Guid aggregateId, int version, DomainEvent @event)
    {
        return new Event
        {
            Id = Guid.NewGuid(),
            AggregateId = aggregateId,
            Version = version,
            Type = @event.GetType().Name,
            Data = JsonSerializer.Serialize(@event)
        };
    }
    public void Save(Guid aggregateId, DomainEvent @event)
    {
        if (_events.ContainsKey(aggregateId))
        {
            var newVersion = _events[aggregateId].Max(e => e.Version) + 1;
            _events[aggregateId].Add(CreateEvent(aggregateId, newVersion, @event));
        }

        _events[aggregateId] = new List<Event> { CreateEvent(aggregateId, 1, @event) };
    }

    private static DomainEvent? DeserializeEvent(Event item) => item.Type switch
    {
        nameof(MatchStarted) => JsonSerializer.Deserialize<MatchStarted>(item.Data),
        nameof(TurnTaken) => JsonSerializer.Deserialize<TurnTaken>(item.Data),
        _ => throw new InvalidOperationException("Type is unknown."),
    };
}
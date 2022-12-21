using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Chess.Core;
using Chess.Core.Match.Events;
using Chess.Domain;

namespace Chess.Application;

public class MatchRepository : IMatchRepository
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
            Type = @event.GetType()?.Name,
            Data = JsonSerializer.Serialize(@event, @event.GetType())
        };
    }
    public void Save(Guid? aggregateId, DomainEvent? @event)
    {
        if (!aggregateId.HasValue) throw new ArgumentNullException(nameof(aggregateId));
        if (@event == null) throw new ArgumentNullException(nameof(@event));

        if (_events.ContainsKey(aggregateId.Value))
        {
            var newVersion = _events[aggregateId.Value].Max(e => e.Version) + 1;
            _events[aggregateId.Value].Add(CreateEvent(aggregateId!.Value, newVersion!.Value, @event));
        }
        else _events[aggregateId.Value] = new List<Event>
        {
            CreateEvent(aggregateId.Value, 1, @event)
        };
    }

    private static DomainEvent? DeserializeEvent(Event item) => item.Type switch
    {
        nameof(MatchStarted) => JsonSerializer.Deserialize<MatchStarted>(item.Data),
        nameof(TurnTaken) => JsonSerializer.Deserialize<TurnTaken>(item.Data),
        _ => throw new InvalidOperationException("Type is unknown."),
    };
}
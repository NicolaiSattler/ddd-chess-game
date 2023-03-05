using Chess.Application;
using Chess.Core;
using Chess.Domain.Aggregates;
using Chess.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Chess.Test;

public class InMemoryMatchRepository : IMatchRepository
{
    private readonly Dictionary<Guid, List<Event>> _events = new();

    public IMatch Get(Guid aggregateId)
    {
        if (_events.TryGetValue(aggregateId, out List<Event> aggregateEvents))
        {
            var events = aggregateEvents.OrderBy(e => e.Version)
                                        .Select(DeserializeEvent);

            var startEvent = events.FirstOrDefault(e => e is MatchStarted) as MatchStarted;

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
    public void Save(Guid aggregateId, DomainEvent @event)
    {
        if (aggregateId == Guid.Empty) throw new ArgumentNullException(nameof(aggregateId));
        if (@event == null) throw new ArgumentNullException(nameof(@event));

        if (_events.ContainsKey(aggregateId))
        {
            var newVersion = _events[aggregateId].Max(e => e.Version) + 1;
            _events[aggregateId].Add(CreateEvent(aggregateId, newVersion!.Value, @event));
        }
        else _events[aggregateId] = new List<Event>
        {
            CreateEvent(aggregateId, 1, @event)
        };
    }

    private static DomainEvent DeserializeEvent(Event item) => item?.Type switch
    {
        nameof(MatchStarted) => JsonSerializer.Deserialize<MatchStarted>(item?.Data ?? string.Empty),
        nameof(TurnTaken) => JsonSerializer.Deserialize<TurnTaken>(item?.Data ?? string.Empty),
        _ => throw new InvalidOperationException("Type is unknown."),
    };
}

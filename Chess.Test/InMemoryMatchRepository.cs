using Chess.Core;
using Chess.Domain.Aggregates;
using Chess.Domain.Events;
using Chess.Infrastructure;
using Chess.Infrastructure.Entity;
using Chess.Infrastructure.Extensions;
using Chess.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chess.Test;

public class InMemoryMatchRepository : IMatchEventRepository
{
    private readonly Dictionary<Guid, List<MatchEvent>> _events = new();

    public async Task<IEnumerable<MatchEvent>> GetAsync(Guid aggregateId)
    {
        if (_events.TryGetValue(aggregateId, out List<MatchEvent> aggregateEvents))
        {
            return aggregateEvents;
        }

        return default;
    }

    private static MatchEvent CreateEvent(Guid aggregateId, int version, DomainEvent @event)
    {
        return new MatchEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = aggregateId,
            Version = version,
            Type = @event.GetType().Name,
            Data = JsonSerializer.Serialize(@event, @event.GetType())
        };
    }
    public async Task AddAsync(Guid aggregateId, DomainEvent @event, bool saveChanges = true)
    {
        if (aggregateId == Guid.Empty) throw new ArgumentNullException(nameof(aggregateId));
        if (@event == null) throw new ArgumentNullException(nameof(@event));

        await Task.CompletedTask;

        if (_events.ContainsKey(aggregateId))
        {
            var newVersion = _events[aggregateId].Max(e => e.Version) + 1;
            _events[aggregateId].Add(CreateEvent(aggregateId, newVersion, @event));
        }
        else _events[aggregateId] = new List<MatchEvent>
        {
            CreateEvent(aggregateId, 1, @event)
        };
    }
}

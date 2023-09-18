using System.Text.Json;
using Chess.Core;
using Chess.Domain.Events;
using Chess.Infrastructure.Entity;

namespace Chess.Infrastructure.Extensions;

public static class MatchEventsExtensions
{
    public static DomainEvent? ToDomainEvent(this MatchEvent @event) => @event.Type switch
    {
        nameof(MatchStarted) => JsonSerializer.Deserialize<MatchStarted>(@event.Data),
        nameof(TurnTaken) => JsonSerializer.Deserialize<TurnTaken>(@event.Data),
        nameof(PawnPromoted) => JsonSerializer.Deserialize<PawnPromoted>(@event.Data),
        nameof(MatchEnded) => JsonSerializer.Deserialize<MatchEnded>(@event.Data),
        _ => throw new InvalidOperationException("Type is unknown."),
    };

}
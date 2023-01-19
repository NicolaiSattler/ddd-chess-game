using Chess.Core;
using Chess.Domain.Aggregates;

namespace Chess.Application;

public interface IMatchRepository
{
    Match? Get(Guid aggregateId);
    void Save(Guid? aggregateId, DomainEvent? @event);
}

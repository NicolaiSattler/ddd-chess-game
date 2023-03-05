using Chess.Core;
using Chess.Domain.Aggregates;

namespace Chess.Application;

public interface IMatchRepository
{
    IMatch Get(Guid aggregateId);
    void Save(Guid aggregateId, DomainEvent @event);

}

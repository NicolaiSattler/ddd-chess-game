using Chess.Core;
using Chess.Core.Match;

namespace Chess.Application;

public interface IMatchRepository
{
    Match? Get(Guid aggregateId);
    void Save(Guid aggregateId, DomainEvent @event);
}

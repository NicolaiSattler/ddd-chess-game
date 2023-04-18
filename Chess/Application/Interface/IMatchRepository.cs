using System.Threading.Tasks;
using Chess.Core;
using Chess.Domain.Aggregates;

namespace Chess.Application;

public interface IMatchRepository
{
    Task<IMatch> GetAsync(Guid aggregateId);
    Task SaveAsync(Guid aggregateId, DomainEvent @event);
}

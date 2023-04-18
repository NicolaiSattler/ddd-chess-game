using System.Threading.Tasks;
using Chess.Domain.Commands;

namespace Chess.Application;

public interface IApplicationService
{
    Task<Guid> StartMatchAsync(StartMatch command);
    Task TakeTurnAsync(Guid aggregateId, TakeTurn command);
    Task ResignAsync(Guid aggregateId, Resign command);
    Task PurposeDrawAsync(Guid aggregateId, ProposeDraw command);
    Task DrawAsync(Guid aggregateId, Draw command);
}


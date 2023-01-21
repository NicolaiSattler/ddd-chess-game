using Chess.Domain.Commands;

namespace Chess.Application;

public interface IApplicationService
{
    Guid StartMatch(StartMatch command);
    void TakeTurn(Guid aggregateId, TakeTurn command);
}


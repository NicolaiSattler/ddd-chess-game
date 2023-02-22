using System.Linq;
using Chess.Domain.Events;
using Chess.Domain.Factories;
using Chess.Domain.Commands;

namespace Chess.Application;

public class ApplicationService : IApplicationService
{
    private readonly IMatchRepository repository;

    public ApplicationService(IMatchRepository repository)
    {
        this.repository = repository;
    }

    public Guid StartMatch(StartMatch command)
    {
        var match = MatchFactory.Create();

        match.Start(command);

        var @event = match.Events.Last();

        if (@event is MatchStarted)
        {
            repository.Save(match.Id, @event);
        }

        return match.Id;
    }

    public void TakeTurn(Guid aggregateId, TakeTurn command)
    {
        var match = repository.Get(aggregateId);
        match.TakeTurn(command);

        var @event = match.Events.Last();

        if (@event != null)
        {
            repository.Save(match.Id, @event);
        }

        if (@event is MatchEnded endEvent)
        {
            //TODO: update elo of players.
        }
    }

    public void Resign(Chess.Domain.Commands.Resign command)
    {

    }
}


using System.Linq;
using Chess.Core.Match.Events;
using Chess.Domain.Factories;
using Chess.Domain.Commands;

namespace Chess.Application;

public class ApplicationService
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

        //publish event?

        var @event = match.Events.Last() as MatchStarted;
        repository.Save(match.Id, @event);

        return match.Id;
    }

    public void TakeTurn(Guid aggregateId, TakeTurn command)
    {
        //Build aggregate
        var match = repository.Get(aggregateId);

        match?.TakeTurn(command);

        // save aggegrate
        repository.Save(match?.Id, match?.Events?.Last());
    }

}


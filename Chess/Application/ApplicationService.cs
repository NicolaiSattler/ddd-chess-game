using System.Linq;
using Chess.Core.Match;
using Chess.Core.Match.Commands;

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
        //Build aggregate
        var match = MatchFactory.Create();

        match.Start(command);

        //publish event

        // save aggegrate
        repository.Save(match.Id, match.Events.Last());

        return match.Id;
    }

    public void TakeTurn(Guid aggregateId, TakeTurn command)
    {
        //Build aggregate
        var match = repository.Get(aggregateId);

        match.TakeTurn(command);

        // save aggegrate
        repository.Save(match.Id, match.Events.Last());
    }

}


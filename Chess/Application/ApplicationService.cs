using Chess.Application.Models;
using Chess.Core;
using Chess.Domain.Aggregates;
using Chess.Domain.Commands;
using Chess.Domain.Events;

using System.Linq;

using ResignCommand = Chess.Domain.Commands.Resign;

namespace Chess.Application;

//TODO: unit test ITurnTimer
public class ApplicationService : IApplicationService
{
    private readonly IMatchRepository _repository;
    private readonly ITurnTimer _timer;

    public ApplicationService(IMatchRepository repository,
                              ITurnTimer timer)
    {
        _repository = repository;
        _timer = timer;
    }

    public Guid StartMatch(StartMatch command)
    {
        var match = new Match(Guid.NewGuid());
        match.Start(command);

        SaveEvent(match);

        return match.Id;
    }

    //TODO: Make sure the timer is not disposed when the instance of ApplicationService is disposing.
    //TODO: result of move should be returned to the end user.
    public void TakeTurn(Guid aggregateId, TakeTurn command)
    {
        _timer.Stop();

        var match = _repository.Get(aggregateId) ?? throw new ApplicationException($"Match could not be found with id {aggregateId}");
        match.TakeTurn(command);

        var @event = SaveEvent(match);

        if (@event is TurnTaken turn)
        {
            var playerAtTurn = command.MemberId == match.White.MemberId ? match.White : match.Black;
            var maxTurnLengthInSeconds = match.Options.MaxTurnTime.Seconds;

            _timer.Start(aggregateId, playerAtTurn!.MemberId, maxTurnLengthInSeconds);
        }
        else if (@event is MatchEnded matchEnd)
        {
            //TODO: update elo of players.
        }
    }

    public void PurposeDraw(Guid aggregateId, ProposeDraw command)
    {
        //TODO: Raise event in UI.
    }

    public void Draw(Guid aggregateId, Draw command)
    {
        _timer.Stop();

        var match = _repository.Get(aggregateId);
        match.Draw(command);

        SaveEvent(match);
    }

    public void Resign(Guid aggregateId, ResignCommand command)
    {
        _timer.Stop();

        var match = _repository.Get(aggregateId);
        match.Resign(command);

        SaveEvent(match);
    }

    private DomainEvent? SaveEvent(IMatch match)
    {
        var @event = match.Events.Last();

        if (@event != null)
        {
            _repository.Save(match.Id, @event);
        }

        return @event;
    }
}


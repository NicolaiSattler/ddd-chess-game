using Chess.Application.Models;
using Chess.Core;
using Chess.Domain.Aggregates;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Application;

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

    public async Task<Guid> StartMatchAsync(StartMatch command)
    {
        var match = new Match(Guid.NewGuid());
        match.Start(command);

        await SaveEventAsync(match);

        return match.Id;
    }

    public async Task<IEnumerable<Piece>> GetPiecesAsync(Guid aggregateId)
    {
        var match = await _repository.GetAsync(aggregateId)
                  ?? throw new ApplicationException($"Match could not be found with id {aggregateId}");
        throw new NotImplementedException();
    }

    //TODO: Make sure the timer is not disposed when the instance of ApplicationService is disposing.
    //TODO: result of move should be returned to the end user.
    public async Task TakeTurnAsync(Guid aggregateId, TakeTurn command)
    {
        _timer.Stop();

        var match = await _repository.GetAsync(aggregateId)
                  ?? throw new ApplicationException($"Match could not be found with id {aggregateId}");
        match.TakeTurn(command);

        var @event = await SaveEventAsync(match);

        if (@event is TurnTaken turn)
        {
            var playerAtTurn = command.MemberId == match.White.MemberId ? match.White : match.Black;
            var maxTurnLengthInSeconds = match.Options.MaxTurnTime.Seconds;

            _timer.Start(aggregateId, playerAtTurn!.MemberId);
        }
        else if (@event is MatchEnded matchEnd)
        {
            //TODO: update elo of players.
        }
    }

    public async Task PurposeDrawAsync(Guid aggregateId, ProposeDraw command)
    {
        throw new NotImplementedException();
        //TODO: Raise event in UI.
    }

    public async Task DrawAsync(Guid aggregateId, Draw command)
    {
        _timer.Stop();

        var match = await _repository.GetAsync(aggregateId);
        match.Draw(command);

        await SaveEventAsync(match);
    }

    public async Task ResignAsync(Guid aggregateId, Resign command)
    {
        _timer.Stop();

        var match = await _repository.GetAsync(aggregateId);
        match.Resign(command);

        await SaveEventAsync(match);
    }

    private async Task<DomainEvent?> SaveEventAsync(IMatch match)
    {
        var @event = match.Events.Last();

        if (@event != null)
        {
            await _repository.SaveAsync(match.Id, @event);
        }

        return @event;
    }
}
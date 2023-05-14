using Chess.Core;
using Chess.Domain.Aggregates;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Events;
using Chess.Infrastructure.Extensions;
using Chess.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Application.Models;

public interface IApplicationService
{
    Task StartMatchAsync(StartMatch command);
    Task TakeTurnAsync(Guid aggregateId, TakeTurn command);
    Task ResignAsync(Guid aggregateId, Resign command);
    Task PurposeDrawAsync(Guid aggregateId, ProposeDraw command);
    Task DrawAsync(Guid aggregateId, Draw command);
    Task<IEnumerable<Piece>> GetPiecesAsync(Guid aggregateId);
}

public class ApplicationService : IApplicationService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchEventRepository _eventRepository;
    private readonly ITurnTimer _timer;

    public ApplicationService(IMatchRepository matchRepository,
                              IMatchEventRepository eventRepository,
                              ITurnTimer turnTimer)
    {
        _matchRepository = matchRepository;
        _eventRepository = eventRepository;
        _timer = turnTimer;
    }

    public async Task StartMatchAsync(StartMatch command)
    {
        var match = new Match(command.AggregateId);
        match.Start(command);

        await SaveEventAsync(match);
    }

    public async Task<IEnumerable<Piece>> GetPiecesAsync(Guid aggregateId)
    {
        var match = await GetAggregateById(aggregateId);
        return match.Pieces;
    }

    //TODO: Make sure the timer is not disposed when the instance of ApplicationService is disposing.
    //TODO: result of move should be returned to the end user.
    //TODO: Piece promotion?
    public async Task TakeTurnAsync(Guid aggregateId, TakeTurn command)
    {
        _timer.Stop();

        var match = await GetAggregateById(aggregateId);

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
        await Task.CompletedTask;
        throw new NotImplementedException();
        //TODO: Raise event in UI.
    }

    public async Task DrawAsync(Guid aggregateId, Draw command)
    {
        _timer.Stop();

        var match = await GetAggregateById(aggregateId);
        match.Draw(command);

        await SaveEventAsync(match);
    }

    public async Task ResignAsync(Guid aggregateId, Resign command)
    {
        _timer.Stop();

        var match = await GetAggregateById(aggregateId);
        match.Resign(command);

        await SaveEventAsync(match);
    }

    private async Task<DomainEvent?> SaveEventAsync(IMatch aggregate)
    {
        var @event = aggregate.Events.Last();

        if (@event is MatchStarted matchStartedEvent)
        {
            await _matchRepository.AddAsync(matchStartedEvent, false);
        }

        await _eventRepository.AddAsync(aggregate.Id, @event);

        return @event;
    }

    private async Task<Match> GetAggregateById(Guid aggregateId)
    {
        var result = await _eventRepository.GetAsync(aggregateId);

        if (!result.Any())
        {
            throw new ApplicationException($"No match events found for {aggregateId}");
        }

        var matchEvents = result.Select(e => e.ToDomainEvent());
        return new(aggregateId, matchEvents);
    }
}
using Chess.Core;
using Chess.Domain.Aggregates;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Events;
using Chess.Domain.Models;
using Chess.Domain.ValueObjects;
using Chess.Infrastructure.Extensions;
using Chess.Infrastructure.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MatchEntity = Chess.Infrastructure.Entity.Match;

namespace Chess.Application.Models;

public interface IApplicationService
{
    Task StartMatchAsync(StartMatch command);
    Task<TurnResult> TakeTurnAsync(Guid aggregateId, TakeTurn command);
    Task ResignAsync(Guid aggregateId, Resign command);
    Task PurposeDrawAsync(Guid aggregateId, ProposeDraw command);
    Task DrawAsync(Guid aggregateId, Draw command);
    Task<List<Piece>> GetPiecesAsync(Guid aggregateId);
    Task<Color> GetColorAtTurnAsync(Guid aggregateId);
    Task<IEnumerable<Turn>> GetTurns(Guid aggregateId);
    Task<IEnumerable<MatchEntity>> GetMatchesAsync();
    Task<Player> GetPlayerAsync(Guid aggregateId, Color color);
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

    public async Task<Color> GetColorAtTurnAsync(Guid aggregateId)
    {
        var match = await GetAggregateById(aggregateId);

        return match.Turns.Last().Player.Color;
    }

    public async Task<List<Piece>> GetPiecesAsync(Guid aggregateId)
    {
        var match = await GetAggregateById(aggregateId);
        return match.Pieces;
    }

    public async Task<Player> GetPlayerAsync(Guid aggregateId, Color color)
    {
        var match = await GetAggregateById(aggregateId);
        return color == Color.White ? match.White : match.Black;
    }

    //TODO: Make sure the timer is not disposed when the instance of ApplicationService is disposing.
    //TODO: Piece promotion?
    public async Task<TurnResult> TakeTurnAsync(Guid aggregateId, TakeTurn command)
    {
        _timer.Stop();

        var match = await GetAggregateById(aggregateId);
        var result = match.TakeTurn(command);

        if (!result.Violations?.Any() ?? false)
        {
            var @event = await SaveEventAsync(match);

            if (@event is TurnTaken)
            {
                var playerAtTurn = command.MemberId == match.White.MemberId ? match.White : match.Black;

                _timer.Start(aggregateId, playerAtTurn!.MemberId);
            }
            else if (@event is MatchEnded)
            {
                //TODO: update elo of players.
                //Make sure elo isn't updated several times.
            }
        }

        return result;
    }

    //TODO: Unit Test
    public async Task PurposeDrawAsync(Guid aggregateId, ProposeDraw command)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
        //TODO: Raise event in UI.
    }

    //TODO: Unit Test
    public async Task<IEnumerable<MatchEntity>> GetMatchesAsync()
    {
        return await _matchRepository.GetAsync();
    }

    //TODO: Unit Test
    public async Task DrawAsync(Guid aggregateId, Draw command)
    {
        _timer.Stop();

        var match = await GetAggregateById(aggregateId);
        match.Draw(command);

        await SaveEventAsync(match);
    }

    //TODO: Unit Test
    public async Task ResignAsync(Guid aggregateId, Resign command)
    {
        _timer.Stop();

        var match = await GetAggregateById(aggregateId);
        match.Resign(command);

        await SaveEventAsync(match);
    }

    public async Task<IEnumerable<Turn>> GetTurns(Guid aggregateId)
    {
        var match = await GetAggregateById(aggregateId);

        return match.Turns.ToList();
    }

    private async Task<DomainEvent?> SaveEventAsync(IMatch aggregate)
    {
        var lastEvent = aggregate.Events.Last();

        if (lastEvent is MatchStarted matchStartedEvent)
        {
            await _matchRepository.AddAsync(matchStartedEvent, false);
        }
        else if (lastEvent is MatchEnded)
        {
            var lastTurnIndex = aggregate.Events.Count() - 2;
            var lastTurnEvent = aggregate.Events.ElementAt(lastTurnIndex);

            await _eventRepository.AddAsync(aggregate.Id, lastTurnEvent, false);
        }

        await _eventRepository.AddAsync(aggregate.Id, lastEvent);

        return lastEvent;
    }

    //TODO: add caching?
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
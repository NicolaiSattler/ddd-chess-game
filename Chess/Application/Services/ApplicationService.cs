using Chess.Application.Events;
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

namespace Chess.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchEventRepository _eventRepository;
    private readonly ITimerService _timer;

    public ApplicationService(IMatchRepository matchRepository,
                              IMatchEventRepository eventRepository,
                              ITimerService turnTimer)
    {
        _matchRepository = matchRepository;
        _eventRepository = eventRepository;

        _timer = turnTimer;
        _timer.TurnExpired += TimerExpiredEventHandler;
    }


    public async Task<Guid> GetPlayerAtTurnAsync(Guid aggregateId)
    {
        var match = await GetAggregateById(aggregateId);

        return match.Turns.Last().Player.MemberId;
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

    //TODO: Unit Test
    public async Task<IEnumerable<MatchEntity>> GetMatchesAsync() => await _matchRepository.GetAsync();

    public async Task<IEnumerable<Turn>> GetTurns(Guid aggregateId)
    {
        var match = await GetAggregateById(aggregateId);

        return match.Turns;
    }

    public async Task StartMatchAsync(StartMatch command)
    {
        var match = new Match(command.AggregateId);
        match.Start(command);

        await SaveEventAsync(match);
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
                var turnTimeInSeconds = (int)match.Options.MaxTurnTime.TotalSeconds;

                _timer.Start(aggregateId, playerAtTurn!.MemberId, turnTimeInSeconds);
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
    public async Task DrawAsync(Guid aggregateId, Draw command)
    {
        _timer.Stop();

        var match = await GetAggregateById(aggregateId);
        match.Draw(command);

        await SaveEventAsync(match);
    }

    //TODO: Unit Test
    public async Task SurrenderAsync(Guid aggregateId, Surrender command)
    {
        _timer.Stop();

        var match = await GetAggregateById(aggregateId);
        match.Surrender(command);

        await SaveEventAsync(match);
    }

    public async Task ForfeitAsync(Guid aggregateId, Forfeit command)
    {
        var match = await GetAggregateById(aggregateId);
        match.Forfeit(command);

        await SaveEventAsync(match);
    }

    public async Task PromotePawnAsync(Guid aggregateId, Square position, PieceType type)
    {
        var match = await GetAggregateById(aggregateId);
        var command = new Promotion() { PawnPosition = position, PromotionType = type };
        match.PromotePiece(command);

        await SaveEventAsync(match);
    }

    private async void TimerExpiredEventHandler(object sender, TurnExpiredEventArgs args)
    {
        var command = new Forfeit { MemberId = args.MemberId };

        await ForfeitAsync(args.AggregateId, command);
    }

    private async Task<DomainEvent?> SaveEventAsync(IMatch aggregate)
    {
        var lastEvent = aggregate.Events.Last();

        if (lastEvent is MatchStarted matchStartedEvent)
        {
            await _matchRepository.AddAsync(matchStartedEvent);
        }
        else if (lastEvent is MatchEnded matchEndedEvent)
        {
            await _eventRepository.AddAsync(aggregate.Id, matchEndedEvent);
            return lastEvent;
        }

        await _eventRepository.AddAsync(aggregate.Id, lastEvent);

        return lastEvent;
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
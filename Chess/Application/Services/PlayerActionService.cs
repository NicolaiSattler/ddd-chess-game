using System.Linq;
using System.Threading.Tasks;
using Chess.Core;
using Chess.Domain.Aggregates;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Events;
using Chess.Domain.Models;
using Chess.Domain.ValueObjects;
using Chess.Infrastructure.Repository;

namespace Chess.Application.Services;

public interface IPlayerActionService
{
    Task<TurnResult> TakeTurnAsync(Guid aggregateId, TakeTurn command);
    Task StartMatchAsync(StartMatch command);
    Task SurrenderAsync(Guid aggregateId, Surrender command);
    Task PurposeDrawAsync(Guid aggregateId, ProposeDraw command);
    Task DrawAsync(Guid aggregateId);
    Task ForfeitAsync(Guid aggregateId, Forfeit command);
    Task PromotePawnAsync(Guid aggregateId, Square position, PieceType type);
}

public class PlayerActionService : IPlayerActionService
{
    private readonly IMatchDataService _dataService;
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchEventRepository _eventRepository;
    private readonly ITimerService _timerService;

    public PlayerActionService(IMatchDataService dataService,
                               IMatchRepository matchRepository,
                               IMatchEventRepository eventRepository,
                               ITimerService timerService)
    {
        _dataService = dataService;
        _matchRepository = matchRepository;
        _eventRepository = eventRepository;
        _timerService = timerService;
    }

    public async Task StartMatchAsync(StartMatch command)
    {
        var match = new Match(command.AggregateId);

        match.Start(command);

        await SaveEventAsync(match);
    }

    public async Task DrawAsync(Guid aggregateId)
    {
        _timerService.Stop();

        var match = await _dataService.GetAggregateAsync(aggregateId);

        match.Draw();

        await SaveEventAsync(match);
    }

    public Task PurposeDrawAsync(Guid aggregateId, ProposeDraw command)
    {
        throw new NotImplementedException();
    }

    public async Task ForfeitAsync(Guid aggregateId, Forfeit command)
    {
        _timerService.Stop();

        var match = await _dataService.GetAggregateAsync(aggregateId);

        match.Forfeit(command);

        await SaveEventAsync(match);
    }

    public async Task PromotePawnAsync(Guid aggregateId, Square position, PieceType type)
    {
        var match = await _dataService.GetAggregateAsync(aggregateId);
        var command = new Promotion() { PawnPosition = position, PromotionType = type };

        match.PromotePiece(command);

        await SaveEventAsync(match);
    }

    public async Task SurrenderAsync(Guid aggregateId, Surrender command)
    {
        _timerService.Stop();

        var match = await _dataService.GetAggregateAsync(aggregateId);
        match.Surrender(command);

        await SaveEventAsync(match);
    }

    public async Task<TurnResult> TakeTurnAsync(Guid aggregateId, TakeTurn command)
    {
        _timerService.Stop();

        var match = await _dataService.GetAggregateAsync(aggregateId);
        var options = match.Options;
        var result = match.TakeTurn(command);

        if (!result.Violations?.Any() ?? false)
        {
            var @event = await SaveEventAsync(match);

            if (@event is TurnTaken)
            {
                var playerAtTurn = command.MemberId == match.White.MemberId ? match.White : match.Black;
                var turnTimeInMilliSeconds = match.Options.MaxTurnTime.TotalMilliseconds;

                if (options.UseTurnTimer)
                    _timerService.Start(aggregateId, playerAtTurn!.MemberId, turnTimeInMilliSeconds);
            }
            else if (@event is MatchEnded)
            {
                //TODO: update elo of players.
                //Make sure elo isn't updated several times.
            }
        }

        return result;
    }

    private async Task<DomainEvent?> SaveEventAsync(IMatch aggregate)
    {
        var lastEvent = aggregate.Events.LastOrDefault();

        if (lastEvent == null)
        {
            return default;
        }
        else if (lastEvent is MatchStarted matchStartedEvent)
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
}
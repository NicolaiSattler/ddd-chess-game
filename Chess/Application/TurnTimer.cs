using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Ardalis.GuardClauses;
using Chess.Core;
using Chess.Domain.Aggregates;
using Chess.Domain.Commands;

namespace Chess.Application.Models;

//TODO: Unit tests
public class TurnTimer : ITurnTimer, IDisposable
{
    private const int MilliSecords = 1000;

    private readonly IMatchRepository _repository;
    private bool _disposing;

    private Timer Timer { get; set; } = new();
    private Guid AggregateId { get; set; }
    private Guid MemberId { get; set; }

    public TurnTimer(IMatchRepository repository)
    {
        _repository = repository;
    }

    public void Start(Guid aggregateId, Guid memberId, int maxTurnLengthInSeconds)
    {
        AggregateId = aggregateId;
        MemberId = memberId;

        var interval = maxTurnLengthInSeconds * MilliSecords;

        Timer = new(maxTurnLengthInSeconds) { Interval = interval };
        Timer.Elapsed += TurnEndedAsync;
        Timer.AutoReset = false;
        Timer.Enabled = true;
    }

    public void Stop()
    {
        Timer.Stop();
        Timer.Dispose();
    }

    public void Dispose() => Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposing)
        {
            if (disposing)
            {
                Timer?.Dispose();
            }

            _disposing = true;
        }
    }

    private async Task<DomainEvent?> SaveEventAsync(IMatch match)
    {
        var @event = match.Events.LastOrDefault();

        if (@event != null)
        {
            await _repository.SaveAsync(match.Id, @event);
        }

        return @event;
    }

    private async void TurnEndedAsync(object? source, ElapsedEventArgs? args)
    {
        var id = Guard.Against.Null<Guid>(AggregateId, nameof(AggregateId));
        var memberId = Guard.Against.Null<Guid>(MemberId, nameof(MemberId));
        var match = await _repository.GetAsync(id);
        var command = new Forfeit() { MemberId = memberId };

        match.Forfeit(command);

        await SaveEventAsync(match);

        Timer.Stop();
    }
}

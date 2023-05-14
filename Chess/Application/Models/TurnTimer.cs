using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Ardalis.GuardClauses;
using Chess.Core;
using Chess.Domain.Aggregates;
using Chess.Domain.Commands;
using Chess.Domain.Configuration;
using Chess.Infrastructure.Extensions;
using Chess.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chess.Application.Models;

public interface ITurnTimer: IHostedService, IDisposable
{
    void Start(Guid aggregateId, Guid memberId);
    void Stop();
}

public class TurnTimer : ITurnTimer
{
    private readonly ILogger<TurnTimer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<MatchOptions> _options;
    private bool _disposing;

    private System.Timers.Timer Timer { get; set; } = new();
    private Guid AggregateId { get; set; }
    private Guid MemberId { get; set; }

    public TurnTimer(ILogger<TurnTimer> logger, IServiceProvider serviceProvider, IOptions<MatchOptions> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var maxTime = _options.Value.MaxTurnTime;
        var milliseconds = 1000;

        Timer = new(maxTime.TotalSeconds) { Interval = maxTime.TotalSeconds * milliseconds };
        Timer.Elapsed += TurnEndedAsync;
        Timer.AutoReset = false;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Stop();

        return Task.CompletedTask;
    }

    public void Dispose() => Dispose(true);

    public void Start(Guid aggregateId, Guid memberId)
    {
        AggregateId = aggregateId;
        MemberId = memberId;

        Timer.Enabled = true;
    }

    public void Stop()
    {
        Timer.Stop();
        Timer.Dispose();
    }

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

    private async void TurnEndedAsync(object? source, ElapsedEventArgs? args)
    {
        var id = Guard.Against.Null<Guid>(AggregateId, nameof(AggregateId));
        var memberId = Guard.Against.Null<Guid>(MemberId, nameof(MemberId));

        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IMatchEventRepository>();

        var result = await repository.GetAsync(id);
        var events = result.Select(e => e.ToDomainEvent());
        var match = new Match(id, events);
        var command = new Forfeit() { MemberId = memberId };

        match.Forfeit(command);

        var @event = match.Events.Last();
        await repository.AddAsync(match.Id, @event);

        Timer.Stop();
    }
}
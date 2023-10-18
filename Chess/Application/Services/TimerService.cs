using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Chess.Application.Events;
using Chess.Domain.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chess.Application.Services;

public interface ITimerService: IHostedService, IAsyncDisposable
{
    void Start(Guid aggregateId, Guid memberId, double remainingTimeInMilliSeconds);
    void Stop();

    event TurnExpiredEventHandler TurnExpired;
}

public class TimerService : ITimerService
{
    private const string TimerExceededMessage = "Turn time expired for member {MemberId} in match {AggregateId}";

    private readonly ILogger<TimerService> _logger;
    private readonly IOptions<MatchOptions> _options;

    public event TurnExpiredEventHandler? TurnExpired;

    private System.Timers.Timer? Timer { get; set; } = new();
    private Guid AggregateId { get; set; }
    private Guid MemberId { get; set; }
    private bool HasMaxTurnTime => _options.Value.MaxTurnTime != TimeSpan.MinValue;

    public TimerService(ILogger<TimerService> logger, IOptions<MatchOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Timer = new() { AutoReset = false };
        Timer.Elapsed += TimerExceeded;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Stop();

        return Task.CompletedTask;
    }

    public void Start(Guid aggregateId, Guid memberId, double remainingTimeInMilliSeconds)
    {
        AggregateId = aggregateId;
        MemberId = memberId;

        if (!HasMaxTurnTime || Timer == null) return;

        Timer.Interval = remainingTimeInMilliSeconds;
        Timer.Start();
    }

    public void Stop() => Timer?.Stop();

    public async ValueTask DisposeAsync()
    {
        if (Timer is IAsyncDisposable timer)
        {
            await timer.DisposeAsync();
        }

        Timer = null;
    }

    private void TimerExceeded(object? source, ElapsedEventArgs? args)
    {
        _logger.LogError(TimerExceededMessage, MemberId, AggregateId);

        TurnExpired?.Invoke(this, new()
        {
            AggregateId = AggregateId,
            MemberId = MemberId,
            ExceededTime = _options.Value.MaxTurnTime
        });

        Stop();
    }
}
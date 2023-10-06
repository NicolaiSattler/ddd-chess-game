using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Chess.Application.Events;
using Chess.Domain.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chess.Application.Services;

public interface ITimerService: IHostedService, IDisposable
{
    void Start(Guid aggregateId, Guid memberId, int remainingTimeInSeconds);
    void Stop();

    event TurnExpiredEventHandler TurnExpired;
}

public class TimerService : ITimerService
{
    private const string TimerExceededMessage = "Turn time expired for member {MemberId} in match {AggregateId}";

    private readonly ILogger<TimerService> _logger;
    private readonly IOptions<MatchOptions> _options;
    private bool _disposing;

    public event TurnExpiredEventHandler? TurnExpired;

    private System.Timers.Timer Timer { get; set; } = new();
    private Guid AggregateId { get; set; }
    private Guid MemberId { get; set; }
    private int RemainingTimeInSeconds { get; set; }
    private bool HasMaxTurnTime => _options.Value.MaxTurnTime != TimeSpan.MinValue;

    public TimerService(ILogger<TimerService> logger,
                     IOptions<MatchOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        const int milliseconds = 1000;

        Timer = new(RemainingTimeInSeconds) { Interval = RemainingTimeInSeconds * milliseconds };
        Timer.Elapsed += TimerExceeded;
        Timer.AutoReset = false;

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Stop();

        return Task.CompletedTask;
    }

    public void Dispose() => Dispose(true);

    public void Start(Guid aggregateId, Guid memberId, int remainingTimeInSeconds)
    {
        AggregateId = aggregateId;
        MemberId = memberId;
        RemainingTimeInSeconds = remainingTimeInSeconds;

        if (!HasMaxTurnTime) return;

        Timer.Start();
    }

    public void Stop() => Timer.Stop();

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

    private void TimerExceeded(object? source, ElapsedEventArgs? args)
    {
        _logger.LogError(TimerExceededMessage, MemberId, AggregateId);

        TurnExpired?.Invoke(this, new()
        {
            MemberId = MemberId,
            ExceededTime = _options.Value.MaxTurnTime
        });

        Stop();
    }
}
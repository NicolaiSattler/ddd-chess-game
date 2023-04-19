using Microsoft.Extensions.Hosting;

namespace Chess.Application;

public interface ITurnTimer: IHostedService, IDisposable
{
    void Start(Guid aggregateId, Guid memberId);
    void Stop();
}


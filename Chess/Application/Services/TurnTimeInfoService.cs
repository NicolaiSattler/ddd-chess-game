using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Chess.Domain.Configuration;
using Chess.Infrastructure.Repository;

namespace Chess.Application.Services;

public interface ITurnTimerInfoService
{
    Task<bool> IsTimeExpiredAsync(Guid aggregateId);
    Task<int> GetRemainingTimeAsync(Guid aggregateId);
}

public class TurnTimerInfoService : ITurnTimerInfoService
{
    private readonly IMatchRepository _matchRepository;

    public TurnTimerInfoService(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public async Task<bool> IsTimeExpiredAsync(Guid aggregateId)
    {
        var remainingTimeInSeconds = await GetRemainingTimeAsync(aggregateId);
        return remainingTimeInSeconds > 0;
    }

    public async Task<int> GetRemainingTimeAsync(Guid aggregateId)
    {
        var result = await _matchRepository.GetAsync(aggregateId, true)
                   ?? throw new ApplicationException($"No match was found for the id {aggregateId}");

        if (!result.Events!.Any()) return 0;

        var lastEvent = result.Events!.Last();
        var matchOptions = JsonSerializer.Deserialize<MatchOptions>(result?.Options ?? string.Empty);

        if (matchOptions?.MaxTurnTime == TimeSpan.MinValue) return 0;

        var deadline = lastEvent.CreatedAtUtc.AddSeconds(matchOptions!.MaxTurnTime.TotalSeconds);
        var timeDifference = deadline - DateTime.UtcNow;

        return (int)timeDifference.TotalSeconds;
    }
}
using System.Linq;
using System.Threading.Tasks;
using Chess.Domain.Events;
using Chess.Domain.ValueObjects;

namespace Chess.Application.Services;

public interface IMatchInfoService
{
    Task<Player> GetPlayerAsync(Guid aggregateId, Color color);
    Task<Guid> GetPlayerAtTurnAsync(Guid aggregateId);
    Task<Color> GetColorAtTurnAsync(Guid aggregateId);
    Task<MatchResult> GetMatchResult(Guid aggregateId);
}

public class MatchInfoService : IMatchInfoService
{
    private readonly IMatchDataService _dataService;

    public MatchInfoService(IMatchDataService dataService)
    {
        _dataService = dataService;
    }
    public async Task<Color> GetColorAtTurnAsync(Guid aggregateId)
    {
        var match = await _dataService.GetAggregateAsync(aggregateId);

        return match.Turns.LastOrDefault()?.Player?.Color ?? Color.Undefined;
    }

    public async Task<MatchResult> GetMatchResult(Guid aggregateId)
    {
        var match = await _dataService.GetAggregateAsync(aggregateId);

        return match.Events?.OfType<MatchEnded>()
                           ?.FirstOrDefault()
                           ?.Result ?? MatchResult.Undefined;
    }

    public async Task<Player> GetPlayerAsync(Guid aggregateId, Color color)
    {
        var match = await _dataService.GetAggregateAsync(aggregateId);

        return color == Color.White ? match.White : match.Black;
    }

    public async Task<Guid> GetPlayerAtTurnAsync(Guid aggregateId)
    {
        var match = await _dataService.GetAggregateAsync(aggregateId);

        return match.Turns.LastOrDefault()?.Player?.MemberId ?? Guid.Empty;
    }
}
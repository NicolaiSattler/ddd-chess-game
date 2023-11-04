using System.Linq;
using System.Threading.Tasks;
using Chess.Domain.Configuration;
using Chess.Domain.Events;
using Chess.Domain.ValueObjects;
using Chess.Infrastructure.Repository;

namespace Chess.Application.Services;

public interface IMatchInfoService
{
    Task<MatchOptions> GetMatchOptionsAsync(Guid aggregateId);
    Task<Player> GetPlayerAsync(Guid aggregateId, Color color);
    Task<Guid> GetPlayerAtTurnAsync(Guid aggregateId);
    Task<Color> GetColorAtTurnAsync(Guid aggregateId);
    Task<MatchResult> GetMatchResult(Guid aggregateId);
}

public class MatchInfoService : IMatchInfoService
{
    private readonly IMatchDataService _dataService;
    private readonly IMatchRepository _matchRepository;

    public MatchInfoService(IMatchDataService dataService, IMatchRepository matchRepository)
    {
        _dataService = dataService;
        _matchRepository = matchRepository;
    }

    public async Task<MatchOptions> GetMatchOptionsAsync(Guid aggregateId)
    {
        var match = await _matchRepository.GetAsync(aggregateId);
        return match.Options;
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
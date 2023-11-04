using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chess.Domain.Aggregates;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Infrastructure.Repository;

using MatchEntity = Chess.Infrastructure.Entity.Match;

namespace Chess.Application.Services;

public interface IMatchDataService
{
    Task<Match> GetAggregateAsync(Guid aggregateId);
    Task<List<Piece>> GetPiecesAsync(Guid aggregateId);
    Task<IEnumerable<Turn>> GetTurns(Guid aggregateId);
    Task<IEnumerable<MatchEntity>> GetMatchesAsync();
}

public class MatchDataService : IMatchDataService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchEventRepository _eventRepository;

    public MatchDataService(IMatchRepository matchRepository, IMatchEventRepository eventRepository)
    {
        _matchRepository = matchRepository;
        _eventRepository = eventRepository;
    }

    //Should the eventstream be played everytime the aggregate is required?
    //Idea: current version should be added to make caching possible.
    public async Task<Match> GetAggregateAsync(Guid aggregateId)
    {
        var result = await _eventRepository.GetAsync(aggregateId);

        if (!result.Any())
        {
            throw new ApplicationException($"No match events found for {aggregateId}");
        }

        var matchEvents = result.Select(e => e.Event).ToList();
        return new(aggregateId, matchEvents);
    }

    public async Task<IEnumerable<MatchEntity>> GetMatchesAsync() => await _matchRepository.GetAsync();

    public async Task<List<Piece>> GetPiecesAsync(Guid aggregateId)
    {
        var match = await GetAggregateAsync(aggregateId);
        return match.Pieces;
    }

    public async Task<IEnumerable<Turn>> GetTurns(Guid aggregateId)
    {
        var match = await GetAggregateAsync(aggregateId);
        return match.Turns;
    }
}
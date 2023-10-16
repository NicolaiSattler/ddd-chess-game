using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Chess.Core;
using Chess.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Chess.Infrastructure.Repository;

public interface IMatchEventRepository
{
    Task<IEnumerable<MatchEvent>> GetAsync(Guid aggregateId);
    Task<MatchEvent> AddAsync(Guid aggregateId, DomainEvent @event, bool saveChanges = true);
}

public class MatchEventRepository : IMatchEventRepository
{
    private readonly ILogger<MatchEventRepository> _logger;
    private readonly IMemoryCache _cache;
    private readonly MatchDbContext _dbContext;

    public MatchEventRepository(ILogger<MatchEventRepository> logger,
                                IMemoryCache cache,
                                MatchDbContext dbContext)
    {
        _logger = logger;
        _cache = cache;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<MatchEvent>> GetAsync(Guid aggregateId)
    {
        try
        {
            var cache = GetFromCache(aggregateId);

            if (cache.Any()) return cache;

            var options = GetCacheOptions();
            var result = await _dbContext.Events!.Where(m => m.AggregateId == aggregateId)
                                                 .OrderBy(m => m.Version)
                                                 .ToListAsync();

            _cache.Set(aggregateId, result, options);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving match with {aggregateId}", aggregateId);

            throw;
        }
    }

    public async Task<MatchEvent> AddAsync(Guid aggregateId, DomainEvent @event, bool saveChanges = true)
    {
        try
        {
            var latestVersion = 0;

            if (_dbContext.Events!.Any(e => e.AggregateId == aggregateId))
            {
                latestVersion = await _dbContext.Events!.Where(m => m.AggregateId == aggregateId)
                                                        .MaxAsync(m => m.Version);
            }

            var matchEvent = new MatchEvent()
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                Version = latestVersion++,
                Type = @event.GetType().Name,
                Data = JsonSerializer.Serialize(@event, @event.GetType(), JsonSerializerOptions.Default),
                CreatedAtUtc = DateTime.UtcNow
            };

            _dbContext.Events!.Add(matchEvent);

            if (saveChanges)
            {
                AddToCache(aggregateId, matchEvent);

                await _dbContext.SaveChangesAsync();
            }

            return matchEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving matchevent for {aggregateId}", aggregateId);

            throw;
        }
    }

    private void AddToCache(Guid aggregateId, MatchEvent matchEvent)
    {
        if (_cache.TryGetValue(aggregateId, out ICollection<MatchEvent>? eventCollection))
        {
            eventCollection ??= new List<MatchEvent>();
            eventCollection.Add(matchEvent);
            var options = GetCacheOptions();
            _cache.Set(aggregateId, eventCollection, options);
        }
    }

    private IEnumerable<MatchEvent> GetFromCache(Guid aggregateId)
    {
        _cache.TryGetValue(aggregateId, out List<MatchEvent>? eventCollection);
        eventCollection ??= new List<MatchEvent>();

        return eventCollection;
    }

    private static MemoryCacheEntryOptions GetCacheOptions() => new()
    {
       AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(30),
    };
}
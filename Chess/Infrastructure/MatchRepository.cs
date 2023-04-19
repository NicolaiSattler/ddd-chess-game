using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Chess.Core;
using Chess.Domain.Aggregates;
using Chess.Infrastructure.Entity;
using Chess.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chess.Infrastructure;

public interface IMatchRepository
{
    Task<IMatch> GetAsync(Guid aggregateId);
    Task SaveAsync(Guid aggregateId, DomainEvent @event);
}

public class MatchRepository : IMatchRepository
{
    private readonly ILogger<MatchRepository> _logger;
    private readonly MatchDbContext _dbContext;

    public MatchRepository(ILogger<MatchRepository> logger, MatchDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IMatch> GetAsync(Guid aggregateId)
    {
        try
        {
            var result = await _dbContext.Events!.Where(m => m.AggregateId == aggregateId)
                                                 .OrderBy(m => m.Version)
                                                 .ToListAsync();

            var events = result.Select(m => m.ToDomainEvent());

            return new Match(aggregateId, events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving match with {aggregateId}", aggregateId);

            throw;
        }
    }

    public async Task SaveAsync(Guid aggregateId, DomainEvent @event)
    {
        try
        {
            var latestVersion = await _dbContext.Events!.Where(m => m.AggregateId == aggregateId)
                                                        .MaxAsync(m => m.Version);
            var matchEvent = new MatchEvent()
            {
                Id = Guid.NewGuid(),
                AggregateId = aggregateId,
                Version = latestVersion++,
                Type = @event.GetType().Name,
                Data = JsonSerializer.Serialize(@event, @event.GetType())
            };

            _dbContext.Events!.Add(matchEvent);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving matchevent for {aggregateId}", aggregateId);

            throw;
        }
    }
}
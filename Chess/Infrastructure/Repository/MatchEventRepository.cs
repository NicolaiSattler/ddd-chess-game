using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Chess.Core;
using Chess.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
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
    private readonly MatchDbContext _dbContext;

    public MatchEventRepository(ILogger<MatchEventRepository> logger, MatchDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<MatchEvent>> GetAsync(Guid aggregateId)
    {
        try
        {
            return await _dbContext.Events!.Where(m => m.AggregateId == aggregateId)
                                           .OrderBy(m => m.Version)
                                           .ToListAsync();
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

            if (_dbContext.Events!.Any())
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
                Data = JsonSerializer.Serialize(@event, @event.GetType())
            };

            _dbContext.Events!.Add(matchEvent);

            if (saveChanges)
            {
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
}
using System.Text.Json;
using System.Threading.Tasks;
using Chess.Domain.Events;
using Chess.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chess.Infrastructure.Repository;

public interface IMatchRepository
{
    Task<Match?> GetAsync(Guid aggregateId);
    Task AddAsync(MatchStarted @event, bool saveChanges);
}

public class MatchRepository : IMatchRepository
{
    private readonly ILogger<MatchRepository> _logger;
    private readonly MatchDbContext _context;

    public MatchRepository(ILogger<MatchRepository> logger, MatchDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Match?> GetAsync(Guid aggregateId)
    {
        try
        {
            return await _context.Matches!.SingleOrDefaultAsync(m => m.AggregateId == aggregateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving match with {aggregateid}", aggregateId);

            throw;
        }
    }

    public async Task AddAsync(MatchStarted @event, bool saveChanges = true)
    {
        try
        {
            var match = new Match
            {
                AggregateId = Guid.NewGuid(),
                BlackPlayerId = @event.BlackMemberId,
                WhitePlayerId = @event.WhiteMemberId,
                StartTime = @event.StartTime,
                Options = JsonSerializer.Serialize(@event.Options)
            };

            _context.Matches!.Add(match);

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving a match for {blackmemberid} and {whitememberid}", @event.BlackMemberId, @event.WhiteMemberId);

            throw;
        }
    }
}
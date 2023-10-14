using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Chess.Domain.Events;
using Chess.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chess.Infrastructure.Repository;

public interface IMatchRepository
{
    Task<Match> GetAsync(Guid aggregateId, bool includeEvents = false);
    Task<IEnumerable<Match>> GetAsync();
    Task<Match?> AddAsync(MatchStarted @event, bool saveChanges = true);
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

    /// <summary>
    /// Retrieve all active matches
    /// </summary>
    public async Task<IEnumerable<Match>> GetAsync() => await _context.Matches.ToListAsync();

    public async Task<Match> GetAsync(Guid aggregateId, bool includeEvents = false)
    {
        try
        {
            if (includeEvents)
            {
                return await _context.Matches
                                     .Include(m => m.Events)
                                     .FirstAsync(m => m.AggregateId == aggregateId)
                    ?? throw new ApplicationException($"No match was found for the id {aggregateId}");
            }
            return await _context.Matches.SingleOrDefaultAsync(m => m.AggregateId == aggregateId)
                ?? throw new ApplicationException($"No match was found for the id {aggregateId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving match with {aggregateid}", aggregateId);

            throw;
        }
    }

    public async Task<Match?> AddAsync(MatchStarted @event, bool saveChanges = true)
    {
        try
        {
            var match = new Match
            {
                AggregateId = @event.AggregateId,
                BlackPlayerId = @event.BlackMemberId,
                WhitePlayerId = @event.WhiteMemberId,
                StartTime = @event.StartTime,
                Options = @event.Options
            };

            _context.Matches!.Add(match);

            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }

            return match;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving a match for {blackmemberid} and {whitememberid}", @event.BlackMemberId, @event.WhiteMemberId);

            throw;
        }
    }
}
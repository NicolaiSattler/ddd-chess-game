using Chess.Core;
using Chess.Domain.Commands;
using Chess.Domain.Configuration;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Events;

[Serializable]
public class MatchStarted : DomainEvent
{
    public Guid AggregateId { get; init; }
    public Guid WhiteMemberId { get; init; }
    public float WhiteElo { get; init; }
    public Guid BlackMemberId { get; init; }
    public float BlackElo { get; init; }
    public DateTime StartTime { get; init; }
    public MatchOptions Options { get; init; } = new();

    public static MatchStarted CreateFrom(StartMatch command, Player white, Player black) => new()
    {
        AggregateId = command.AggregateId,
        WhiteMemberId = white.MemberId,
        WhiteElo = white.Elo,
        BlackMemberId = black.MemberId,
        BlackElo = black.Elo,
        StartTime = DateTime.UtcNow,
        Options = command.Options
    };
}

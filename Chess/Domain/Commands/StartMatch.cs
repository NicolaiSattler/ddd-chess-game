using Chess.Domain.Configuration;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Commands;

public record StartMatch
{
    public Guid AggregateId { get; init; }
    public Player MemberOne { get; init; } = new();
    public Player MemberTwo { get; init; } = new();
    public MatchOptions Options { get; init; } = new();
}

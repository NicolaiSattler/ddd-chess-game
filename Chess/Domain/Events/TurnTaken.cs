using Chess.Core;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Events;

public class TurnTaken : DomainEvent
{
    public Guid MemberId { get; init; }
    public Square StartPosition { get; init; } = Square.Empty();
    public Square EndPosition { get; init; } = Square.Empty();
    public DateTime EndTime { get; init; }

    public static TurnTaken CreateFrom(TakeTurn command) => new()
    {
        MemberId = command.MemberId,
        StartPosition = command.StartPosition,
        EndPosition = command.EndPosition,
        EndTime = DateTime.UtcNow
    };
}

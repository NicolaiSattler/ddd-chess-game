using System;
using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match.Events;

public class TakeTurn : DomainEvent
{
    public Guid MemberId { get; init; }
    public Guid PieceId { get; init; }
    public Square NewSquare { get; init; }
}
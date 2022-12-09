using System;
using DDD.Core.Game.ValueObjects;

namespace DDD.Core.Game.ValueObjects;

public record Player
{
    public Guid MemberId { get; init; }
    public Color Color { get; init; }
}
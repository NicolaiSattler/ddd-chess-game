using System;
using DDD.Core.Game.ValueObjects;

namespace DDD.Core.Game;

public class Player : Entity<Guid>
{
    public Color Color { get; init; }

    public Player(Guid id, Color color) : base(id)
    {
        Color = color;
    }
}
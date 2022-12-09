using System;
using DDD.Core.Game.ValueObjects;

namespace DDD.Core.Game.Entities;

public class Move: Entity<Guid>
{
    public Piece CurrentPiece { get; init;}
    public Square NewPosition { get; private set; }
    public DateTime StartTime { get; init; }
    public Player Player { get; init; }

    public Move(Guid id) : base(id) { }
}
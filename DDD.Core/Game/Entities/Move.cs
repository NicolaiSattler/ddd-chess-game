using System;
using DDD.Core.Game.ValueObjects;

namespace DDD.Core.Game.Entities;

public class Move: Entity<Guid>
{
    public Move(Guid id) : base(id) { }

    public Piece CurrentPiece { get; init;}
    public Position NewPosition { get; private set; }
    public DateTime StartTime { get; init; }
}
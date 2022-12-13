using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match.Entities;

public class Move: Entity<Guid>
{
    public Piece CurrentPiece { get; init;}
    public Square NewPosition { get; private set; }
    public DateTime StartTime { get; init; }
    public Player Player { get; init; }

    public Move() : base(Guid.NewGuid()) { }
}
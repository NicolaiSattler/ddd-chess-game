using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match.Entities;

public class Move : Entity<Guid>
{
    public Piece Piece { get; set; }
    public Square NewPosition { get; set; }
    public DateTime StartTime { get; init; }
    public Player Player { get; init; }

    public Move() : base(Guid.NewGuid()) { }
}

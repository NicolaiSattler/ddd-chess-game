using Chess.Core;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities;

public class Move : Entity<Guid>
{
    public Piece Piece { get; set; }
    public Square? NewPosition { get; set; }
    public DateTime StartTime { get; init; }
    public Player? Player { get; init; }

    public Move() : base(Guid.NewGuid()) { }
}

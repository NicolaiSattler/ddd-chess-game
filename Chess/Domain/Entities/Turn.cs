using Chess.Core;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Entities;

public class Turn : Entity
{
    public PieceType? PieceType { get; set; }
    public Square? StartPosition { get; set; }
    public Square? EndPosition { get; set; }
    //Startime in UTC
    public DateTime StartTime { get; init; }
    public Player? Player { get; init; }

    public Turn() : base(Guid.NewGuid()) { }
}

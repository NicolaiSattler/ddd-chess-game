using Chess.Core;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Events;

public class PawnPromoted: DomainEvent
{
    public Square PawnPosition { get; init; } = new(File.Undefined, 0);
    public PieceType PromotionType { get; init; }
}
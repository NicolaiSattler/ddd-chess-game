using Chess.Core;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Events;

public class PawnPromoted: DomainEvent
{
    public Square PawnPosition { get; init; } = new(File.Undefined, 0);
    public PieceType PromotionType { get; init; }

    public static PawnPromoted CreateFrom(Promotion command) => new()
    {
        PawnPosition = command.PawnPosition,
        PromotionType = command.PromotionType
    };
}